using BlApi.Exceptions;
using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlApi
{
    /// <summary>
    /// Buisnes layer class, extends IBL
    /// </summary>
    internal partial class BL : IBL
    {
        private List<DroneForList> BLdrones = new();
        internal DalApi.IDAL Idal = DalFactory.GetDal();
        internal double[] elecRate;
        private static Random rand = new();

        /// <summary>
        /// Constructor for BL object
        /// </summary>
        /// <exception cref="BlException"></exception>
        public BL()
        {
            elecRate = Idal.GetElectricity();
            foreach (DO.Drone DALdrone in Idal.GetAllDrones())
            {
                DroneForList BLdrone = new();
                BLdrone.Id = DALdrone.Id;
                BLdrone.Model = DALdrone.Model;
                BLdrone.Weight = (WeightGroup)((int)DALdrone.Weight);

                bool inCharge = Idal.isInCharge(BLdrone.Id); // check if the drone already in charge

                DO.Package? AssociatedButNotDelivered;
                try
                {
                    AssociatedButNotDelivered = (Idal.GetAllPackages().
                        Where(p => (p.DroneId == (int?)BLdrone.Id &&
                        (p.Delivered is null)))).First();
                }
                catch (InvalidOperationException)
                {
                    AssociatedButNotDelivered = null;
                }
                if (AssociatedButNotDelivered is not null && !inCharge)
                {
                    BLdrone.State = DroneState.Busy;
                    BLdrone.PassingPckageId = AssociatedButNotDelivered?.Id;

                    bool WasPickedUp = AssociatedButNotDelivered?.PickUp is not null;
                    //no need here for a try block, as we know that the senderId exists
                    DO.Customer customer = Idal.GetCustomer((int)AssociatedButNotDelivered?.SenderId);
                    Location customerLoc = new(customer.Longitude, customer.Lattitude);
                    if (WasPickedUp)
                    {
                        BLdrone.CurrentLocation = customerLoc;
                    }
                    else // accosiatied but not picked. the closet station to the sender
                    {
                        int? stationId = GetClosetStation(new(customer.Longitude, customer.Lattitude), false);
                        DO.Station s = GetDALStation((int)stationId);
                        BLdrone.CurrentLocation = new(s.Longitude, s.Lattitude);
                    }
                    // add batery
                    double minBattery = BatteryToDeliver((DO.Package)AssociatedButNotDelivered, BLdrone);
                    if (minBattery > 100)//the minumun battery needed for the delivery is larger than 100% charge
                    {
                        throw new BlException("Not enough free stations!", BLdrone.Id, typeof(Drone));
                    }

                    BLdrone.Battery = rand.NextDouble() * (100 - minBattery) + minBattery;
                }

                else
                {
                    int state = rand.Next(0, 2);
                    if (state == 0 || inCharge) // Maitenance
                    {
                        BLdrone.State = DroneState.Maitenance;

                        BLdrone.Battery = ((double)rand.Next(1, 21));

                        var stations = Idal.GetAllStationsWhere(s => s.ChargeSlots > 0);
                        var station = stations.ElementAt(rand.Next(0, stations.Count()));
                        BLdrone.CurrentLocation = new(station.Longitude, station.Lattitude);
                        if (!inCharge)
                        {
                            Idal.SendDroneToCharge(BLdrone.Id, station.Id);
                        }
                    }
                    else //empty
                    {
                        BLdrone.State = DroneState.Empty;
                        var allPackages = Idal.GetAllPackages().Where(p => p.Delivered is not null);
                        if (allPackages.Count() != 0)
                        {
                            var customer = GetDALCustomer(allPackages.ToList()[rand.Next(0, allPackages.Count())].RecevirId);
                            BLdrone.CurrentLocation = new(customer.Longitude, customer.Lattitude);
                        }
                        else
                        {
                            var customer = Idal.GetAllCustomers().ToList()[0];
                            BLdrone.CurrentLocation = new(customer.Longitude, customer.Lattitude);
                        }
                        int? id = GetClosetStation(BLdrone.CurrentLocation);
                        if (id is null)
                        {
                            throw new BlException("No free charging stations", BLdrone.Id, typeof(Drone));
                        }

                        var station = GetDALStation((int)id);
                        Location stationLoc = new(station.Longitude, station.Lattitude);
                        double minBattery = DistanceTo(stationLoc, BLdrone.CurrentLocation) / ElecOfDrone(BLdrone);
                        if (minBattery > 100)
                        {
                            throw new BlException("Not enough free stations!", BLdrone.Id, typeof(Drone));
                        }
                        BLdrone.Battery = rand.NextDouble() * (100 - minBattery) + minBattery;
                    }
                }
                BLdrones.Add(BLdrone);
            }

        }

        /// <summary>
        /// Adds s to the data layer
        /// </summary>
        /// <param name="s">
        /// Station to add
        /// </param>
        /// <exception cref="ObjectAllreadyExistsException"></exception>
        public void AddStation(Station s)
        {
            try
            {
                Idal.AddStation(s.Id, s.Name, s.LocationOfStation.Longitude, s.LocationOfStation.Latitude, s.ChargingDrones.Count() + s.AmountOfEmptyPorts); ;
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        /// <summary>
        /// Adds <paramref name="d"/> to the data layer
        /// </summary>
        /// <param name="d">
        /// Drone to add
        /// </param>
        /// <exception cref="ObjectAllreadyExistsException"></exception>
        public void AddDrone(Drone d)
        {
            try
            {
                Idal.AddDrone(d.Id, d.Model, (DO.WeightGroup)((int)d.Weight));

                DroneForList df = new DroneForList(d.Id, d.Model, d.Weight, d.Battery, DroneState.Empty, d.CurrentLocation, d.Package.Id);
                BLdrones.Add(df);
                if (d.State == DroneState.Maitenance)
                    SendDroneToCharge(d.Id);
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        /// <summary>
        /// Adds <paramref name="c"/> to the data layer
        /// </summary>
        /// <param name="c">
        /// Customer to add
        /// </param>
        /// <exception cref="ObjectAllreadyExistsException"></exception>
        public void AddCustomer(Customer c)
        {
            try
            {
                Idal.AddCustomer(c.Id, c.Name, c.PhoneNumber, c.CustomerLocation.Latitude, c.CustomerLocation.Longitude);
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        /// <summary>
        /// Adds <paramref name="p"/> to the data layer
        /// </summary>
        /// <param name="p">
        /// Package to add
        /// </param>
        /// <exception cref="ObjectAllreadyExistsException"></exception>
        public void AddPackage(Package p)
        {
            try
            {
                Idal.AddPackage(p.Sender.Id, p.Reciver.Id, (DO.WeightGroup)((int)p.Weight), ((DO.Priority)(int)p.Priority), null, null, null, null, null);
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        /// <summary>
        /// Adds a new package to the data layer
        /// </summary>
        /// <param name="sid">
        /// sender id to add
        /// </param>
        /// <param name="sid">
        /// sender id to add
        /// </param>
        public void AddPackage(int sid, int rid, WeightGroup w, PriorityGroup p)
        {

            Idal.AddPackage(sid, rid, (DO.WeightGroup)((int)w), ((DO.Priority)(int)p), null, null, null, null, null);

        }

        /// <summary>
        /// Changes the drone with <paramref name="id"/>'s model name to <paramref name="newModel"/>
        /// </summary>
        /// <param name="id">
        /// Id of the wanted drone to change
        /// </param>
        /// <param name="newModel">
        /// New model name
        /// </param>
        public void UpdateDroneName(int id, string newModel, double newBattery=-1 , Location newLocation=null)
        {
            DO.Drone DALdrone = GetDALDrone(id);
            newModel = newModel == "" ? DALdrone.Model : newModel;

            DALdrone.Model = newModel;

            Idal.UpdateDrone(DALdrone);

            IEnumerable<DroneForList> myDrone = BLdrones.Where(d => d.Id == id);
            if (myDrone.Count() == 1)
            {
                DroneForList BLdrone = myDrone.ElementAt(0);
                BLdrone.Model = DALdrone.Model;
                if (newBattery >= 0)
                    BLdrone.Battery = newBattery;
                if (newLocation is not null)
                    BLdrone.CurrentLocation = newLocation;
            }
        }

        /// <summary>
        /// Update the data in the <c>Station</c> wehre ID equals <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <param name="newChargeSlots"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateStation(int id, string newName = "", int newEmptyChargeSlots = -1)
        {
            DO.Station station = GetDALStation(id);
            Station s = GetStationById(id);
            //int totalPorts = s.AmountOfEmptyPorts + s.ChargingDrones.Count();

            station.Name = newName != "" ? newName : station.Name;

            if (newEmptyChargeSlots < 0)
            {
                newEmptyChargeSlots = station.ChargeSlots;
            }
            else
            {
                station.ChargeSlots = newEmptyChargeSlots;
            }

            Idal.UpdateStation(station);
        }

        /// <summary>
        /// Update the data in the <c>Customer</c> wehre ID equals <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <param name="newPhone"></param>
        public void UpdateCustomer(int id, string newName = "", string newPhone = "")
        {
            DO.Customer customer = GetDALCustomer(id);
            customer.Name = newName != "" ? newName : customer.Name;
            customer.Phone = newPhone != "" ? newPhone : customer.Phone;
            Idal.UpdateCustomer(customer);
        }

        /// <summary>
        /// Sends the <c>Drone</c> with ID <paramref name="DroneId"/> to charge in the closest station with open ports.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <exception cref="BlException"></exception>
        /// <exception cref="ObjectDoesntExistException"></exception>
        /// <exception cref="DroneStateException"></exception>
        public void SendDroneToCharge(int DroneId)
        {
            DroneForList drone = BLdrones.Find(d => d.Id == DroneId);
            if (drone is not null && drone.State == DroneState.Empty)
            {
                int? ClosestId = GetClosetStation(drone.CurrentLocation);
                if (ClosestId is not null)
                {
                    DO.Station closest = GetDALStation((int)ClosestId);
                    double distance = DistanceTo(new(closest.Longitude, closest.Lattitude), drone.CurrentLocation);
                    double batteryNeed = distance / ElecOfDrone(drone);
                    if (batteryNeed <= drone.Battery)
                    {
                        // we can send the drone for charging!!
                        drone.Battery -= batteryNeed;
                        drone.CurrentLocation = new(closest.Longitude, closest.Lattitude);
                        drone.State = DroneState.Maitenance;

                        Idal.SendDroneToCharge(drone.Id, closest.Id);

                    }
                    else
                    {
                        throw new BlException($"there are no emtpy stations that {DroneId} has enogth battery to fly them! need {batteryNeed} has {drone.Battery}");
                    }
                }

                else
                {
                    throw new BlException($"there are no empty stations for {DroneId}!");
                }
            }
            else
            {
                if (drone is null)
                {
                    throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
                }

                throw new DroneStateException($"the drone {DroneId} can send to charge only if it empty! currently {drone.State}");
            }
        }

        /// <summary>
        /// Releases the <c>Drone</c> with ID <paramref name="DroneId"/> from charging as if <paramref name="time"/> minutes have passed.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <param name="time"></param>
        /// <exception cref="DroneStateException"></exception>
        /// <exception cref="ObjectDoesntExistException"></exception>
        public void ReleaseDrone(int DroneId, DateTime outTime)
        {
            DO.Drone DALdrone = GetDALDrone(DroneId);
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);

            if (BLdrone is not null)
            {
                if (BLdrone.State == DroneState.Maitenance)
                {

                    double time = Idal.ReleaseDroneFromCharge(DroneId, outTime, -1); // find the station id by yourelf, via the DroneCharges object
                    // returns time in second from the start of the charge
                    BLdrone.Battery += elecRate[4] * time;
                    BLdrone.Battery = BLdrone.Battery > 100 ? 100 : BLdrone.Battery;
                    BLdrone.State = DroneState.Empty;
                }
                else
                {
                    throw new DroneStateException($"the drone {DroneId} can be release only if it state is in Maitenance! currently {BLdrone.State}");
                }
            }
            else
            {
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
            }

        }

        /// <summary>
        /// Assigns a package to the <c>Drone</c> with ID <paramref name="DroneId"/>.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <exception cref="BlException"></exception>
        /// <exception cref="DroneStateException"></exception>
        /// <exception cref="ObjectDoesntExistException"></exception>
        public void AssignPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);
            if (BLdrone is not null)
            {
                if (BLdrone.State == DroneState.Empty)
                {
                    IEnumerable<DO.Package> allPackages = Idal.GetAllPackagesWhere(p => p.DroneId is null);

                    IEnumerable<DO.Package> allCanWeight = allPackages.Where(p => (int)p.Weight <= (int)BLdrone.Weight);

                    if (allCanWeight.Count() > 0)
                    {
                        IEnumerable<DO.Package> allNear = allCanWeight.Where(p => DroneHaveEnoughBattery(p, BLdrone));

                        List<DO.Package> allNearList = allNear.ToList();
                        if (allNearList.Count() > 0)
                        {
                            allNearList.Sort((p1, p2) => PackagePriority(p1, p2, BLdrone.CurrentLocation));

                            int PackageId = allNearList[0].Id;

                            BLdrone.State = DroneState.Busy;
                            BLdrone.PassingPckageId = PackageId;

                            Idal.GivePackageDrone(PackageId, BLdrone.Id); // change drone id in package, change time assosiating
                        }
                        else
                        {
                            throw new BlException($"there are no free package for the drone {DroneId}", DroneId, typeof(Drone));
                        }
                    }
                    else
                    {
                        throw new BlException($"there are no free package for the drone {DroneId}", DroneId, typeof(Drone));
                    }

                }
                else
                {
                    throw new DroneStateException($"the drone {DroneId} cannot be associated because it is not empty! currently {BLdrone.State}");
                }
            }
            else
            {
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
            }
        }

        /// <summary>
        /// Gives the <c>Drone</c> with <paramref name="DroneId"/> the package it is assigned.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="DroneStateException"></exception>
        public void PickUpPackage(int DroneId, bool simulatorMode = false)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId);

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.PassingPckageId is null ?
                    throw new ObjectDoesntExistException($"The drone with ID {DroneId} is not paired to a package!") :
                    (int)BLdrone.PassingPckageId;
                DO.Package p = GetDALPackage(PackageId);
                if (p.Associated is not null && p.PickUp is null)
                {
                    // the battery was checked in associate

                    DO.Customer Sender = GetDALCustomer(p.SenderId);
                    Location SenderLoc = new(Sender.Longitude, Sender.Lattitude);
                    if (!simulatorMode)
                    {
                        double batteryNeed = (1/elecRate[0]) * DistanceTo(BLdrone.CurrentLocation, SenderLoc);
                        if (batteryNeed > BLdrone.Battery)
                            throw new BlException($"not enougth battery of {BLdrone.Id}, need {batteryNeed}, has {BLdrone.Battery}");
                        BLdrone.Battery -= batteryNeed;
                    }

                    BLdrone.CurrentLocation = SenderLoc;
                    Idal.PickUpPackage(PackageId); // update time

                }
                else
                {
                    throw new BlException("the package is not associated or picked up", PackageId, p.GetType());
                }

            }
            else
            {
                throw new DroneStateException($"cannot pick up with drone that is not busy! the current state is {BLdrone.State}");
            }

        }

        /// <summary>
        /// Releases the package from the <c>Drone</c> with the ID <paramref name="DroneId"/>
        /// </summary>
        /// <param name="DroneId"></param>
        public void DeliverPackage(int DroneId, bool simulatorMode = false)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId); // replace it with get by id

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.PassingPckageId is null ?
                    throw new ObjectDoesntExistException($"The drone with ID {DroneId} is not paired to a package!") :
                    (int)BLdrone.PassingPckageId;
                DO.Package p = GetDALPackage(PackageId);
                if (p.PickUp is not null && p.Delivered is null)
                {
                    // the battery was checked in associate

                    DO.Customer Recv = Idal.GetCustomer(p.RecevirId);
                    Location RecvLoc = new(Recv.Longitude, Recv.Lattitude);

                    if (!simulatorMode)
                    {
                        double batteryNeed = 1 / ElecOfDrone(BLdrone) * DistanceTo(BLdrone.CurrentLocation, RecvLoc);
                        if (batteryNeed > BLdrone.Battery)
                            throw new BlException($"not enougth battery of {BLdrone.Id}, need {batteryNeed}, has {BLdrone.Battery}");

                        BLdrone.Battery -= batteryNeed;
                    }



                    BLdrone.CurrentLocation = RecvLoc;
                    BLdrone.State = DroneState.Empty;

                    BLdrone.PassingPckageId = null;

                    p.Delivered = DateTime.Now;
                    Idal.UpdatePackage(p);
                }
                else
                {
                    throw new BlException($"the package {PackageId} state is not \" pick up\"!");
                }

            }
            else
            {
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
            }
        }

        /// <summary>
        /// Gets the <c>Station</c> that has the ID <paramref name="StationId"/>.
        /// </summary>
        /// <param name="StationId"></param>
        /// <returns>BO.Sattion</returns>
        public Station GetStationById(int StationId)
        {
            var DALstation = GetDALStation(StationId);
            List<DroneInCharging> charged = new();

            foreach (DroneForList BLDrone in BLdrones)
            {

                if (BLDrone.State == DroneState.Maitenance && BLDrone.CurrentLocation.Latitude == DALstation.Lattitude && BLDrone.CurrentLocation.Longitude == DALstation.Longitude)
                {
                    charged.Add(new DroneInCharging(BLDrone.Id, BLDrone.Battery));
                }
            }

            return new Station(DALstation.Id, DALstation.Name, new(DALstation.Longitude, DALstation.Lattitude), DALstation.ChargeSlots, charged);
        }

        /// <summary>
        /// Gets the <c>Drone</c> that has the ID <paramref name="DroneId"/>.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <returns>BO.Drone</returns>
        public Drone GetDroneById(int DroneId)
        {
            var DALdrone = GetDALDrone(DroneId);
            DroneForList droneForList = BLdrones.Find(d => d.Id == DroneId);

            PackageInTransfer pckTransfer = null;

            if (droneForList.State == DroneState.Busy)
            {
                int pkgId = (int)droneForList.PassingPckageId;
                var DALpkg = GetDALPackage(pkgId);

                var DALsend = GetDALCustomer(DALpkg.SenderId);
                var DALrecv = GetDALCustomer(DALpkg.RecevirId);

                CustomerForPackage send = new CustomerForPackage(DALpkg.SenderId, DALsend.Name);
                CustomerForPackage recv = new CustomerForPackage(DALpkg.RecevirId, DALrecv.Name);

                Location locSend = new(DALsend.Longitude, DALsend.Lattitude), locRecv = new(DALrecv.Longitude, DALrecv.Lattitude);

                bool inDelivery = DALpkg.PickUp is not null;
                pckTransfer = new PackageInTransfer(pkgId, inDelivery, (WeightGroup)(int)DALpkg.Weight, (PriorityGroup)(int)DALpkg.PackagePriority, send, recv, locSend, locRecv, inDelivery? DistanceTo(droneForList.CurrentLocation, locRecv) : DistanceTo(droneForList.CurrentLocation, locSend));
            }

            return new Drone(droneForList.Id, droneForList.Model, droneForList.Weight, droneForList.Battery, droneForList.State, pckTransfer, droneForList.CurrentLocation);
        }

        /// <summary>
        /// Gets the <c>Customer</c> that has the ID <paramref name="CustomerId"/>.
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns>BO.Customer</returns>
        public Customer GetCustomerById(int CustomerId)
        {

            var DALcustomer = GetDALCustomer(CustomerId);

            List<PackageForCustomer> pkgFrom = new();
            List<PackageForCustomer> pkgTo = new();

            IEnumerable<DO.Package> DALpackages = Idal.GetAllPackages();

            foreach (DO.Package dp in DALpackages)
            {
                if (dp.Associated is not null) // if the package is in the progress of delivering
                {

                    if (dp.SenderId == CustomerId || dp.RecevirId == CustomerId)
                    {
                        PackageStatus status = dp.Delivered is not null ? PackageStatus.Accepted : (dp.PickUp is not null ? PackageStatus.PickedUp : dp.Associated is not null ? PackageStatus.Paired : PackageStatus.Initialized);

                        PackageForCustomer pkgForCustomer = new(dp.Id, (WeightGroup)((int)dp.Weight), (PriorityGroup)((int)dp.PackagePriority), status, new CustomerForPackage(CustomerId, DALcustomer.Name));

                        if (dp.SenderId == CustomerId)
                        {
                            pkgFrom.Add(pkgForCustomer);
                        }
                        else // the Customer is the reciver of this pakcage
                        {
                            pkgTo.Add(pkgForCustomer);
                        }
                    }
                }
            }

            return new Customer(DALcustomer.Id, DALcustomer.Name, DALcustomer.Phone, new(DALcustomer.Longitude, DALcustomer.Lattitude), pkgFrom, pkgTo);
        }

        /// <summary>
        /// Gets the <c>Package</c> that has the ID <paramref name="PackageId"/>.
        /// </summary>
        /// <param name="PackageId"></param>
        /// <returns>BO.Package</returns>
        public Package GetPackageById(int PackageId)
        {
            var DALpkg = GetDALPackage(PackageId);

            Package ans = new Package();

            WeightGroup weight = (WeightGroup)((int)DALpkg.Weight);
            PriorityGroup priority = (PriorityGroup)((int)DALpkg.PackagePriority);

            ans.Id = PackageId;
            ans.Weight = weight;
            ans.Priority = priority;

            var DALsender = GetDALCustomer(DALpkg.SenderId);
            CustomerForPackage sender = new CustomerForPackage(DALsender.Id, DALsender.Name);

            var DALrecv = GetDALCustomer(DALpkg.RecevirId);
            CustomerForPackage recv = new CustomerForPackage(DALrecv.Id, DALrecv.Name);

            ans.Sender = sender;
            ans.Reciver = recv;

            ans.TimeToPackage = DALpkg.Created;

            if (DALpkg.DroneId.HasValue)
            {
                DroneForList BLdrone = BLdrones.Find(d => d.Id == DALpkg.DroneId);
                DroneForPackage drone = new DroneForPackage(BLdrone.Id, BLdrone.Battery, BLdrone.CurrentLocation);
                ans.Drone = drone;

                ans.TimeToPair = DALpkg.Associated;
                ans.TimeToPickup = DALpkg.PickUp;
                ans.TimeToDeliver = DALpkg.Delivered;
            }
            return ans;
        }

        /// <summary>
        /// Returns a list of all stations
        /// </summary>
        /// <returns>List of BO.StationForList</returns>
        public IEnumerable<StationForList> GetAllStations()
        {
            List<StationForList> ret = new();
            foreach (var station in Idal.GetAllStations())
            {
                Station blStation = GetStationById(station.Id);
                ret.Add(new StationForList(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count()));
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of all drones
        /// </summary>
        /// <returns>List of BO.DroneForList</returns>
        public IEnumerable<DroneForList> GetAllDrones()
        {
            List<DroneForList> ret = new();
            foreach (var drone in Idal.GetAllDrones())
            {
                Drone blDrone = GetDroneById(drone.Id);
                ret.Add(new(blDrone.Id, blDrone.Model, blDrone.Weight, blDrone.Battery, blDrone.State, blDrone.CurrentLocation, (blDrone.Package is null ? -1 : blDrone.Package.Id)));
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of all customers
        /// </summary>
        /// <returns>List of BO.CustomerForList</returns>
        public IEnumerable<CustomerForList> GetAllCustomers()
        {
            List<CustomerForList> ret = new();
            foreach (var customer in Idal.GetAllCustomers())
            {
                Customer blCustomer = GetCustomerById(customer.Id);
                int sentAccepted = Idal.GetAllPackages().Where(p => p.SenderId == blCustomer.Id &&
                p.Delivered is null).Count();
                int sentOnTheWay = Idal.GetAllPackages().Where(p => p.SenderId == blCustomer.Id &&
                p.Delivered is not null).Count();
                int accepted = Idal.GetAllPackages().Where(p => p.RecevirId == blCustomer.Id &&
                p.Delivered is null).Count();
                int onTheWay = Idal.GetAllPackages().Where(p => p.RecevirId == blCustomer.Id &&
                p.Delivered is not null).Count();
                ret.Add(new CustomerForList(blCustomer.Id, blCustomer.Name, blCustomer.PhoneNumber, sentAccepted,
                    sentOnTheWay, accepted, onTheWay));
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of all Packages
        /// </summary>
        /// <returns>List of BO.PackageForList</returns>
        public IEnumerable<PackageForList> GetAllPackages()
        {
            List<PackageForList> ret = new();
            foreach (var package in Idal.GetAllPackages())
            {
                Package blPackage = GetPackageById(package.Id);
                ret.Add(new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority, GetPackageState(blPackage)));
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of all unpaired packages
        /// </summary>
        /// <returns>List of BO.PackageForList</returns>
        public IEnumerable<PackageForList> GetPackagesWithoutDrone()
        {
            List<PackageForList> ret = new();
            foreach (var package in Idal.GetAllPackagesWhere(p => p.DroneId is null))
            {
                Package blPackage = GetPackageById(package.Id);
                ret.Add(new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority, PackageStatus.Initialized));
            }
            return ret;
        }

        /// <summary>
        /// Returns a list of all stations with open charging ports
        /// </summary>
        /// <returns>List of BO.StationForList</returns>
        public IEnumerable<StationForList> GetStationsWithCharges()
        {
            List<StationForList> ret = new();
            foreach (var station in Idal.GetAllStationsWhere(s => s.ChargeSlots > 0))
            {
                Station blStation = GetStationById(station.Id);
                ret.Add(new(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count()));
            }
            return ret;
        }

        #region Private dal acces
        private DO.Station GetDALStation(int StationId)
        {
            try
            {
                return Idal.GetStation(StationId);
            }
            catch (ArgumentException e)
            {
                throw new ObjectDoesntExistException(e.Message);
            }
        }

        private DO.Drone GetDALDrone(int DroneId)
        {
            try
            {
                return Idal.GetDrone(DroneId);
            }
            catch (ArgumentException e)
            {
                throw new ObjectDoesntExistException(e.Message);
            }
        }

        private DO.Package GetDALPackage(int PackageId)
        {
            try
            {
                return Idal.GetPackage(PackageId);

            }
            catch (ArgumentException e)
            {
                throw new ObjectDoesntExistException(e.Message);
            }
        }

        private DO.Customer GetDALCustomer(int CustomerId)
        {
            try
            {
                return Idal.GetCustomer(CustomerId);

            }
            catch (ArgumentException e)
            {
                throw new ObjectDoesntExistException(e.Message);
            }
        }
        #endregion

        #region Utility
        private int PackagePriority(DO.Package p1, DO.Package p2, Location DroneLoc)
        {
            if ((int)p1.PackagePriority > (int)p2.PackagePriority)
            {
                return 1;
            }

            if ((int)p1.PackagePriority < (int)p2.PackagePriority)
            {
                return -1;
            }

            if ((int)p1.Weight > (int)p2.Weight)
            {
                return 1;
            }

            if ((int)p1.Weight < (int)p2.Weight)
            {
                return -1;
            }

            DO.Customer p1Cust = Idal.GetCustomer(p1.SenderId);
            DO.Customer p2Cust = Idal.GetCustomer(p2.SenderId);

            Location LocP1 = new(p1Cust.Longitude, p1Cust.Lattitude);
            Location LocP2 = new(p2Cust.Longitude, p2Cust.Lattitude);

            double p1Dist = DistanceTo(DroneLoc, LocP1);
            double p2Dist = DistanceTo(DroneLoc, LocP2);

            if (p1Dist > p2Dist)
            {
                return -1;
            }

            if (p1Dist < p2Dist)
            {
                return 1;
            }

            return 0;
        }

        private double DistanceToDoDeliver(DO.Package p, DroneForList d)
        {
            var sender = GetDALCustomer(p.SenderId);
            Location senderLoc = new(sender.Longitude, sender.Lattitude);

            var recv = GetDALCustomer(p.RecevirId);
            Location recvLoc = new(recv.Longitude, recv.Lattitude);

            int? StationId = GetClosetStation(recvLoc);
            if (StationId is null)
            {
                /**
             * ATTENTION: now,if all the stations have no charging slot, the associationg cannot be!!! 
             */
                return double.PositiveInfinity;
            }
            DO.Station closest = GetDALStation((int)StationId);
            Location closestLoc = new(closest.Longitude, closest.Lattitude);

            double distance = DistanceTo(d.CurrentLocation, senderLoc) + DistanceTo(recvLoc, senderLoc) + DistanceTo(closestLoc, recvLoc);

            return distance;
        }

        private bool DroneHaveEnoughBattery(DO.Package p, DroneForList d)
        {
            
            return BatteryToDeliver(p,d) <= d.Battery;
        }

        private double BatteryToDeliver(DO.Package p , DroneForList d)
        {
            var sender = GetDALCustomer(p.SenderId);
            Location senderLoc = new(sender.Longitude, sender.Lattitude);

            var recv = GetDALCustomer(p.RecevirId);
            Location recvLoc = new(recv.Longitude, recv.Lattitude);

            int? StationId = GetClosetStation(recvLoc);
            if (StationId is null)
            {
                /**
             * ATTENTION: now,if all the stations have no charging slot, the associationg cannot be!!! 
             */
                return double.PositiveInfinity;
            }
            DO.Station closest = GetDALStation((int)StationId);
            Location closestLoc = new(closest.Longitude, closest.Lattitude);

            double battery = (1/elecRate[0]) * DistanceTo(d.CurrentLocation, senderLoc) + (1/elecRate[(int)p.Weight]) * DistanceTo(recvLoc, senderLoc) + (1/elecRate[0]) * DistanceTo(closestLoc, recvLoc);

            return battery;
        }


        internal static double DistanceTo(Location Loc1, Location Loc2, char unit = 'K')
        {
            double lat1 = Loc1.Latitude; double lon1 = Loc1.Longitude;
            double lat2 = Loc2.Latitude; double lon2 = Loc2.Longitude;

            if (lat1 == lat2 && lon1 == lon2)
                return 0;

            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case 'K': //Kilometers -> default
                    return dist * 1.609344;
                case 'N': //Nautical Miles 
                    return dist * 0.8684;
                case 'M': //Miles
                    return dist;
            }

            return dist;
        }


        internal int? GetClosetStation(Location loc, bool toCharge = true)
        {
            IEnumerable<DO.Station> FreeStations = Idal.GetAllStations();
            if (toCharge)
            {
                FreeStations = FreeStations.Where(s => s.ChargeSlots > 0);
            }

            if (FreeStations.Count() > 0)
            {
                DO.Station closest = FreeStations.Aggregate((s1, s2) => DistanceTo(new(s1.Longitude, s1.Lattitude), loc)
               > DistanceTo(new(s2.Longitude, s2.Lattitude), loc)
               ? s2 : s1);

                return closest.Id;
            }
            else
            {
                return null;
            }
        }


        internal double ElecOfDrone(int Id)
        {
            return ElecOfDrone(GetAllDrones().Where(d => d.Id == Id).First());
        }

        private double ElecOfDrone(DroneForList d)
        {
            double[] elec = this.elecRate;
            int ix = -1;
            if (d.State == DroneState.Empty)
            {
                ix = 0;
            }
            else if (d.State == DroneState.Busy)
            {
                switch (Idal.GetPackage((int)d.PassingPckageId).Weight)
                {
                    case DO.WeightGroup.Light:
                        ix = 1;
                        break;
                    case DO.WeightGroup.Mid:
                        ix = 2;
                        break;
                    case DO.WeightGroup.Heavy:
                        ix = 3;
                        break;
                }
            }
            else
            {
                throw new InvalidOperationException($"The dorne {d.Id} is in maitenance");
            }

            return elec[ix];
        }

        private PackageStatus GetPackageState(Package p)
        {

            return p.TimeToDeliver is not null ?
                   PackageStatus.Accepted :
                   (p.TimeToPickup is not null ?
                   PackageStatus.PickedUp :
                   (p.TimeToPair is not null ?
                   PackageStatus.Paired :
                   PackageStatus.Initialized));
        }


        #endregion

        public void StartSimulator(int DroneId, Action update, Func<bool> stop)
        {
            new Simulator(this, DroneId, update, stop);
        }

        public IEnumerable<dynamic> GetObjectsWhere<T>(Func<T, bool> func)
        {
            IEnumerable<dynamic> listRet = typeof(T).Name.Replace("ForList", "") switch
            {
                "Drone" => GetAllDrones(),
                "Station" => GetAllStations(),
                "Customer" => GetAllCustomers(),
                "Package" => GetAllPackages(),
                _ => throw new InvalidOperationException()
            };
            foreach (var item in listRet)
                if (func(typeof(T).Name.Contains("ForList") ? item : GetObject(item.Id, item.GetType().Name)))
                    yield return item;

        }

        private dynamic GetObject(int id, string typeOf)
        {
            return typeOf switch
            {
                "Drone" => GetDroneById(id),
                "Station" => GetStationById(id),
                "Customer" => GetCustomerById(id),
                "Package" => GetPackageById(id),
                _ => throw new InvalidOperationException()
            };
        }
    }
}
