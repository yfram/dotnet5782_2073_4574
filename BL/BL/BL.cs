// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using BlApi.Exceptions;
using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlApi
{
    /// <summary>
    /// Business layer class, extends IBL
    /// </summary>
    internal partial class BL : IBL
    {
        private readonly List<DroneForList> BLdrones = new();
        internal IDAL Idal = DalFactory.GetDal();
        internal double[] elecRate;
        private static readonly Random random = new();

        /// <summary>
        /// Constructor for BL object
        /// </summary>
        /// <exception cref="BlException"></exception>
        public BL()
        {
            elecRate = Idal.GetElectricity();
            foreach (DO.Drone DALdrone in Idal.GetAllDrones())
            {
                DroneForList bLdrone = new();
                bLdrone.Id = DALdrone.Id;
                bLdrone.Model = DALdrone.Model;
                bLdrone.Weight = (WeightGroup)((int)DALdrone.Weight);

                bool inCharge = Idal.isInCharge(bLdrone.Id);
                DO.Package? associatedButNotDelivered;
                try
                {
                    associatedButNotDelivered = (Idal.GetAllPackages().
                        Where(p => (p.DroneId == (int?)bLdrone.Id &&
                        (p.Delivered is null)))).First();
                }
                catch (InvalidOperationException)
                {
                    associatedButNotDelivered = null;
                }
                if (associatedButNotDelivered is not null && !inCharge)
                {
                    bLdrone.State = DroneState.Busy;
                    bLdrone.PassingPckageId = associatedButNotDelivered?.Id;
                    DO.Customer customer = Idal.GetCustomer((int)associatedButNotDelivered?.SenderId);
                    Location customerLoc = new(customer.Longitude, customer.Latitude);
                    if (associatedButNotDelivered?.PickUp is not null)
                        bLdrone.CurrentLocation = customerLoc;
                    else // associated but not picked. the closet station to the sender
                    {
                        int? stationId = GetClosetStation(new(customer.Longitude, customer.Latitude), false);
                        DO.Station s = GetDALStation((int)stationId);
                        bLdrone.CurrentLocation = new(s.Longitude, s.Latitude);
                    }
                    // add battery
                    double minBattery = BatteryToDeliver((DO.Package)associatedButNotDelivered, bLdrone);
                    if (minBattery > 100)//the minimum battery needed for the delivery is larger than 100% charge
                        throw new BlException("Not enough free stations!", bLdrone.Id, typeof(Drone));

                    bLdrone.Battery = random.NextDouble() * (100 - minBattery) + minBattery;
                }
                else
                {
                    int state = random.Next(0, 2);
                    if (state == 0 || inCharge) // Maintenance
                    {
                        bLdrone.State = DroneState.Maitenance;
                        bLdrone.Battery = ((double)random.Next(1, 21));

                        var stations = Idal.GetAllStationsWhere(s => s.ChargeSlots > 0);
                        var station = stations.ElementAt(random.Next(0, stations.Count()));
                        bLdrone.CurrentLocation = new(station.Longitude, station.Latitude);
                        if (!inCharge)
                            Idal.SendDroneToCharge(bLdrone.Id, station.Id);
                    }
                    else //Empty
                    {
                        bLdrone.State = DroneState.Empty;
                        var allPackages = Idal.GetAllPackages().Where(p => p.Delivered is not null);
                        if (allPackages.Any())
                        {
                            var customer = GetDALCustomer(allPackages.ToList()[random.Next(0, allPackages.Count())].RecevirId);
                            bLdrone.CurrentLocation = new(customer.Longitude, customer.Latitude);
                        }
                        else
                        {
                            var customer = Idal.GetAllCustomers().ToList()[0];
                            bLdrone.CurrentLocation = new(customer.Longitude, customer.Latitude);
                        }
                        int? id = GetClosetStation(bLdrone.CurrentLocation);
                        if (id is null)
                            throw new BlException("No free charging stations", bLdrone.Id, typeof(Drone));

                        var station = GetDALStation((int)id);
                        Location stationLoc = new(station.Longitude, station.Latitude);
                        double minBattery = DistanceTo(stationLoc, bLdrone.CurrentLocation) / ElecOfDrone(bLdrone);
                        if (minBattery > 100)
                            throw new BlException("Not enough free stations!", bLdrone.Id, typeof(Drone));

                        bLdrone.Battery = random.NextDouble() * (100 - minBattery) + minBattery;
                    }
                }
                BLdrones.Add(bLdrone);
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
                Idal.AddStation(s.Id, s.Name, s.LocationOfStation.Longitude,
                    s.LocationOfStation.Latitude, s.ChargingDrones.Count() + s.AmountOfEmptyPorts);
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

                DroneForList df = new(d.Id, d.Model, d.Weight,
                    d.Battery, DroneState.Empty, d.CurrentLocation, d.Package.Id);
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
                Idal.AddCustomer(c.Id, c.Name, c.PhoneNumber,
                    c.CustomerLocation.Latitude, c.CustomerLocation.Longitude);
            }
            catch (ArgumentException e)
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
                Idal.AddPackage(p.Sender.Id, p.Reciver.Id, (DO.WeightGroup)((int)p.Weight),
                    ((DO.Priority)(int)p.Priority), null, null, null, null, null);
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
        /// Changes the drone with ID <paramref name="id"/>'s model name to <paramref name="newModel"/>
        /// </summary>
        /// <param name="id">
        /// Id of the wanted drone to change
        /// </param>
        /// <param name="newModel">
        /// New model name
        /// </param>
        public void UpdateDroneName(int id, string newModel, double newBattery = -1, Location newLocation = null)
        {
            DO.Drone DALdrone = GetDALDrone(id);
            DALdrone.Model = newModel == "" ? DALdrone.Model : newModel;
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
        /// Update the data in the <c>Station</c> where ID equals <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <param name="newChargeSlots"></param>
        /// <exception cref="ArgumentException"></exception>
        public void UpdateStation(int id, string newName = "", int newEmptyChargeSlots = -1)
        {
            DO.Station station = GetDALStation(id);
            station.Name = newName != "" ? newName : station.Name;
            station.ChargeSlots = newEmptyChargeSlots == -1 ? station.ChargeSlots : newEmptyChargeSlots;
            Idal.UpdateStation(station);
        }

        /// <summary>
        /// Update the data in the <c>Customer</c> where ID equals <paramref name="id"/>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <param name="newPhone"></param>
        public void UpdateCustomer(int id, string newName = "", string newPhone = "")
        {
            DO.Customer customer = GetDALCustomer(id);
            customer.Name = newName != "" ? newName : customer.Name;
            customer.Phone = newPhone != "" ? newPhone : customer.Phone;
            try
            {
                Idal.UpdateCustomer(customer);
            }
            catch (ArgumentException)
            {
                throw new ObjectDoesntExistException();
            }
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
                int? closestId = GetClosetStation(drone.CurrentLocation);
                if (closestId is not null)
                {
                    DO.Station closest = GetDALStation((int)closestId);
                    double distance = DistanceTo(new(closest.Longitude, closest.Latitude), drone.CurrentLocation);
                    double batteryNeed = distance / ElecOfDrone(drone);
                    if (batteryNeed <= drone.Battery)
                    {
                        drone.Battery -= batteryNeed;
                        drone.CurrentLocation = new(closest.Longitude, closest.Latitude);
                        drone.State = DroneState.Maitenance;
                        Idal.SendDroneToCharge(drone.Id, closest.Id);
                    }
                    else
                    {
                        throw new BlException($"There are no empty stations that {DroneId} has enough battery to fly to!" +
                            $" It needs {batteryNeed}% but only has {drone.Battery}%");
                    }
                }
                else
                    throw new BlException($"there are no empty stations for {DroneId}!");
            }
            else
            {
                if (drone is null)
                    throw new ObjectDoesntExistException($"the drone {DroneId} does not exist!");

                throw new DroneStateException($"A drone can only be sent to charge if it is empty!The drone {drone.Id} is currently {drone.State}");
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
            DO.Drone dALdrone = GetDALDrone(DroneId);
            DroneForList bLdrone = BLdrones.Find(d => d.Id == DroneId);

            if (bLdrone is not null)
            {
                if (bLdrone.State == DroneState.Maitenance)
                {

                    double time = Idal.ReleaseDroneFromCharge(DroneId, outTime, -1);
                    bLdrone.Battery = (elecRate[4] * time + bLdrone.Battery) > 100 ? 100 : elecRate[4] * time + bLdrone.Battery;
                    bLdrone.State = DroneState.Empty;
                }
                else
                    throw new DroneStateException($"the drone {DroneId} can be release only if it's already charging! It is currently {bLdrone.State}");
            }
            else
                throw new ObjectDoesntExistException($"the drone {DroneId} does not exist!");
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
            DroneForList bLdrone = BLdrones.Find(d => d.Id == DroneId);
            if (bLdrone is not null)
            {
                if (bLdrone.State == DroneState.Empty)
                {
                    IEnumerable<DO.Package> allPackages = Idal.GetAllPackagesWhere(p => p.DroneId is null);
                    IEnumerable<DO.Package> allCanWeight = allPackages.Where(p => (int)p.Weight <= (int)bLdrone.Weight);

                    if (allCanWeight.Any())
                    {
                        IEnumerable<DO.Package> allNear = allCanWeight.Where(p => DroneHaveEnoughBattery(p, bLdrone));

                        List<DO.Package> allNearList = allNear.ToList();
                        if (allNearList.Count > 0)
                        {
                            allNearList.Sort((p1, p2) => PackagePriority(p1, p2, bLdrone.CurrentLocation));

                            int PackageId = allNearList[0].Id;

                            bLdrone.State = DroneState.Busy;
                            bLdrone.PassingPckageId = PackageId;

                            Idal.GivePackageDrone(PackageId, bLdrone.Id); // change drone id in package, change time associating
                        }
                        else
                            throw new BlException($"there are no free packages for the drone {DroneId}", DroneId, typeof(Drone));
                    }
                    else
                        throw new BlException($"there are no free packages for the drone {DroneId}", DroneId, typeof(Drone));
                }
                else
                    throw new DroneStateException($"the drone {DroneId} cannot be associated because it is not empty! currently {bLdrone.State}");
            }
            else
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exist!");
        }

        /// <summary>
        /// Gives the <c>Drone</c> with <paramref name="DroneId"/> the package it is assigned.
        /// </summary>
        /// <param name="DroneId"></param>
        /// <exception cref="ObjectDoesntExistException"></exception>
        /// <exception cref="BlException"></exception>
        /// <exception cref="DroneStateException"></exception>
        public void PickUpPackage(int DroneId, bool simulatorMode = false)
        {
            DroneForList bLdrone = BLdrones.First(d => d.Id == DroneId);

            if (bLdrone.State == DroneState.Busy)
            {
                int PackageId = bLdrone.PassingPckageId is null ?
                    throw new ObjectDoesntExistException($"The drone with ID {DroneId} is not paired to a package!") :
                    (int)bLdrone.PassingPckageId;
                DO.Package p = GetDALPackage(PackageId);
                if (p.Associated is not null && p.PickUp is null)
                {
                    DO.Customer Sender = GetDALCustomer(p.SenderId);
                    Location SenderLoc = new(Sender.Longitude, Sender.Latitude);
                    if (!simulatorMode)
                    {
                        double batteryNeed = (1 / elecRate[0]) * DistanceTo(bLdrone.CurrentLocation, SenderLoc);
                        if (batteryNeed > bLdrone.Battery)
                            throw new BlException($"Not enough battery of {bLdrone.Id}, need {batteryNeed}, has {bLdrone.Battery}");

                        bLdrone.Battery -= batteryNeed;
                    }
                    bLdrone.CurrentLocation = SenderLoc;
                    Idal.PickUpPackage(PackageId);
                }
                else
                    throw new BlException("The package is not associated or picked up", PackageId, p.GetType());
            }
            else
                throw new DroneStateException($"Cannot pick up with drone that is not busy! the current state is {bLdrone.State}");
        }

        /// <summary>
        /// Releases the package from the <c>Drone</c> with the ID <paramref name="droneId"/>
        /// </summary>
        /// <param name="droneId"></param>
        public void DeliverPackage(int droneId, bool simulatorMode = false)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == droneId);

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.PassingPckageId is null ?
                    throw new ObjectDoesntExistException($"The drone with ID {droneId} is not paired to a package!") :
                    (int)BLdrone.PassingPckageId;
                DO.Package p = GetDALPackage(PackageId);
                if (p.PickUp is not null && p.Delivered is null)
                {
                    DO.Customer Recv = Idal.GetCustomer(p.RecevirId);
                    Location RecvLoc = new(Recv.Longitude, Recv.Latitude);
                    if (!simulatorMode)
                    {
                        double batteryNeed = 1 / ElecOfDrone(BLdrone) * DistanceTo(BLdrone.CurrentLocation, RecvLoc);
                        if (batteryNeed > BLdrone.Battery)
                            throw new BlException($"Drone {BLdrone.Id} does not have enough battery, it needs {batteryNeed}%, but has {BLdrone.Battery}%");

                        BLdrone.Battery -= batteryNeed;
                    }
                    BLdrone.CurrentLocation = RecvLoc;
                    BLdrone.State = DroneState.Empty;

                    BLdrone.PassingPckageId = null;

                    p.Delivered = DateTime.Now;
                    Idal.UpdatePackage(p);
                }
                else
                    throw new BlException($"The package {PackageId}'s state is not \"pick up\"!");
            }
            else
                throw new ObjectDoesntExistException($"the drone {droneId} is not exist!");
        }

        /// <summary>
        /// Gets the <c>Station</c> that has the ID <paramref name="StationId"/>.
        /// </summary>
        /// <param name="StationId"></param>
        /// <returns>BO.Sattion</returns>
        public Station GetStationById(int StationId)
        {
            var dALstation = GetDALStation(StationId);
            List<DroneInCharging> charged = new();
            foreach (DroneForList bLDrone in BLdrones)
            {
                if (bLDrone.State == DroneState.Maitenance && bLDrone.CurrentLocation.Latitude == dALstation.Latitude && bLDrone.CurrentLocation.Longitude == dALstation.Longitude)
                    charged.Add(new DroneInCharging(bLDrone.Id, bLDrone.Battery));
            }

            return new Station(dALstation.Id, dALstation.Name, new(dALstation.Longitude, dALstation.Latitude), dALstation.ChargeSlots, charged);
        }

        /// <summary>
        /// Gets the <c>Drone</c> that has the ID <paramref name="droneId"/>.
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns>BO.Drone</returns>
        public Drone GetDroneById(int droneId)
        {
            var dALdrone = GetDALDrone(droneId);
            DroneForList droneForList = BLdrones.Find(d => d.Id == droneId);
            PackageInTransfer pckTransfer = null;

            if (droneForList.State == DroneState.Busy)
            {
                int pkgId = (int)droneForList.PassingPckageId;
                var DALpkg = GetDALPackage(pkgId);

                var DALsend = GetDALCustomer(DALpkg.SenderId);
                var DALrecv = GetDALCustomer(DALpkg.RecevirId);

                CustomerForPackage send = new(DALpkg.SenderId, DALsend.Name);
                CustomerForPackage recv = new(DALpkg.RecevirId, DALrecv.Name);

                Location locSend = new(DALsend.Longitude, DALsend.Latitude), locRecv = new(DALrecv.Longitude, DALrecv.Latitude);

                bool inDelivery = DALpkg.PickUp is not null;
                pckTransfer = new PackageInTransfer(pkgId, inDelivery, (WeightGroup)(int)DALpkg.Weight, (PriorityGroup)(int)DALpkg.PackagePriority, send, recv, locSend, locRecv, inDelivery ? DistanceTo(droneForList.CurrentLocation, locRecv) : DistanceTo(droneForList.CurrentLocation, locSend));
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
            var dALcustomer = GetDALCustomer(CustomerId);

            List<PackageForCustomer> pkgFrom = new();
            List<PackageForCustomer> pkgTo = new();
            IEnumerable<DO.Package> dALpackages = Idal.GetAllPackages();

            foreach (DO.Package dp in dALpackages)
            {
                if (dp.Associated is not null) // if the package is in the process of delivering
                {
                    if (dp.SenderId == CustomerId || dp.RecevirId == CustomerId)
                    {
                        PackageStatus status = dp.Delivered is not null ? PackageStatus.Accepted : (dp.PickUp is not null ? PackageStatus.PickedUp :
                            dp.Associated is not null ? PackageStatus.Paired : PackageStatus.Initialized);
                        PackageForCustomer pkgForCustomer = new(dp.Id, (WeightGroup)(int)dp.Weight,
                            (PriorityGroup)(int)dp.PackagePriority, status,
                            new CustomerForPackage(CustomerId, dALcustomer.Name));

                        if (dp.SenderId == CustomerId)
                            pkgFrom.Add(pkgForCustomer);
                        else
                            pkgTo.Add(pkgForCustomer);
                    }
                }
            }

            return new Customer(dALcustomer.Id, dALcustomer.Name, dALcustomer.Phone,
                new(dALcustomer.Longitude, dALcustomer.Latitude), pkgFrom, pkgTo);
        }

        /// <summary>
        /// Gets the <c>Package</c> that has the ID <paramref name="PackageId"/>.
        /// </summary>
        /// <param name="PackageId"></param>
        /// <returns>BO.Package</returns>
        public Package GetPackageById(int PackageId)
        {
            var dALpkg = GetDALPackage(PackageId);
            Package ans = new();

            WeightGroup weight = (WeightGroup)(int)dALpkg.Weight;
            PriorityGroup priority = (PriorityGroup)((int)dALpkg.PackagePriority);

            ans.Id = PackageId;
            ans.Weight = weight;
            ans.Priority = priority;

            var dALsender = GetDALCustomer(dALpkg.SenderId);
            CustomerForPackage sender = new(dALsender.Id, dALsender.Name);

            var DALrecv = GetDALCustomer(dALpkg.RecevirId);
            CustomerForPackage recv = new(DALrecv.Id, DALrecv.Name);

            ans.Sender = sender;
            ans.Reciver = recv;

            ans.TimePackaged = dALpkg.Created;

            if (dALpkg.DroneId.HasValue)
            {
                DroneForList BLdrone = BLdrones.Find(d => d.Id == dALpkg.DroneId);
                DroneForPackage drone = new(BLdrone.Id, BLdrone.Battery, BLdrone.CurrentLocation);
                ans.Drone = drone;

                ans.TimePaired = dALpkg.Associated;
                ans.TimePickedUp = dALpkg.PickUp;
                ans.TimeDeliverd = dALpkg.Delivered;
            }
            return ans;
        }

        /// <summary>
        /// Returns a list of all stations
        /// </summary>
        /// <returns>List of BO.StationForList</returns>
        public IEnumerable<StationForList> GetAllStations()
        {
            foreach (var station in Idal.GetAllStations())
            {
                Station blStation = GetStationById(station.Id);
                yield return new StationForList(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count());
            }
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
                yield return new CustomerForList(blCustomer.Id, blCustomer.Name, blCustomer.PhoneNumber, sentAccepted,
                    sentOnTheWay, accepted, onTheWay);
            }
        }

        /// <summary>
        /// Returns a list of all Packages
        /// </summary>
        /// <returns>List of BO.PackageForList</returns>
        public IEnumerable<PackageForList> GetAllPackages()
        {
            foreach (var package in Idal.GetAllPackages())
            {
                Package blPackage = GetPackageById(package.Id);
                yield return new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority, GetPackageState(blPackage));
            }
        }

        /// <summary>
        /// Returns a list of all unpaired packages
        /// </summary>
        /// <returns>List of BO.PackageForList</returns>
        public IEnumerable<PackageForList> GetPackagesWithoutDrone()
        {
            foreach (var package in Idal.GetAllPackagesWhere(p => p.DroneId is null))
            {
                Package blPackage = GetPackageById(package.Id);
                yield return new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority, PackageStatus.Initialized);
            }
        }

        /// <summary>
        /// Returns a list of all stations with open charging ports
        /// </summary>
        /// <returns>List of BO.StationForList</returns>
        public IEnumerable<StationForList> GetStationsWithCharges()
        {
            foreach (var station in Idal.GetAllStationsWhere(s => s.ChargeSlots > 0))
            {
                Station blStation = GetStationById(station.Id);
                yield return new(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count());
            }
        }

        #region Private dal access
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
                return 1;

            if ((int)p1.PackagePriority < (int)p2.PackagePriority)
                return -1;

            if ((int)p1.Weight > (int)p2.Weight)
                return 1;

            if ((int)p1.Weight < (int)p2.Weight)
                return -1;

            DO.Customer p1Cust = Idal.GetCustomer(p1.SenderId);
            DO.Customer p2Cust = Idal.GetCustomer(p2.SenderId);

            Location LocP1 = new(p1Cust.Longitude, p1Cust.Latitude);
            Location LocP2 = new(p2Cust.Longitude, p2Cust.Latitude);

            double p1Dist = DistanceTo(DroneLoc, LocP1);
            double p2Dist = DistanceTo(DroneLoc, LocP2);

            return p1Dist > p2Dist ? -1 : p1Dist < p2Dist ? 1 : 0;
        }

        private bool DroneHaveEnoughBattery(DO.Package p, DroneForList d)
        {
            return BatteryToDeliver(p, d) <= d.Battery;
        }

        private double BatteryToDeliver(DO.Package p, DroneForList d)
        {
            var sender = GetDALCustomer(p.SenderId);
            Location senderLoc = new(sender.Longitude, sender.Latitude);

            var recv = GetDALCustomer(p.RecevirId);
            Location recvLoc = new(recv.Longitude, recv.Latitude);

            int? stationId = GetClosetStation(recvLoc);
            if (stationId is null)
                return double.PositiveInfinity;

            DO.Station closest = GetDALStation((int)stationId);
            Location closestLoc = new(closest.Longitude, closest.Latitude);

            double battery = (1 / elecRate[0]) * DistanceTo(d.CurrentLocation, senderLoc) +
                (1 / elecRate[(int)p.Weight]) * DistanceTo(recvLoc, senderLoc) +
                (1 / elecRate[0]) * DistanceTo(closestLoc, recvLoc);
            return battery;
        }

        internal static double DistanceTo(Location Loc1, Location Loc2, char unit = 'K')
        {
            double lat1 = Loc1.Latitude;
            double lon1 = Loc1.Longitude;
            double lat2 = Loc2.Latitude;
            double lon2 = Loc2.Longitude;

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
            return unit switch
            {
                'K' => dist * 1.609344,
                'N' => dist * 0.8684,
                'M' => dist,
                _ => throw new NotSupportedException($"Unit type '{unit}' not supported yet"),
            };
        }

        internal int? GetClosetStation(Location loc, bool toCharge = true)
        {
            IEnumerable<DO.Station> freeStations = Idal.GetAllStations();
            if (toCharge)
                freeStations = freeStations.Where(s => s.ChargeSlots > 0);

            if (freeStations.Any())
            {
                DO.Station closest = freeStations.Aggregate((s1, s2) => DistanceTo(new(s1.Longitude, s1.Latitude), loc) >
                    DistanceTo(new(s2.Longitude, s2.Latitude), loc) ?
                    s2 : s1);
                return closest.Id;
            }
            return null;
        }

        internal double ElecOfDrone(int Id)
        {
            return ElecOfDrone(GetAllDrones().Where(d => d.Id == Id).First());
        }

        private double ElecOfDrone(DroneForList d)
        {
            int ix = -1;
            if (d.State is DroneState.Empty)
                ix = 0;
            else if (d.State is DroneState.Busy)
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
                throw new InvalidOperationException($"The drone {d.Id} is in maintenance");

            return elecRate[ix];
        }

        private static PackageStatus GetPackageState(Package p)
        {

            return p.TimeDeliverd is not null ?
                   PackageStatus.Accepted :
                   (p.TimePickedUp is not null ?
                   PackageStatus.PickedUp :
                   (p.TimePaired is not null ?
                   PackageStatus.Paired :
                   PackageStatus.Initialized));
        }
        #endregion

        public void StartSimulator(int DroneId, Action update, Func<bool> stop)
        {
            _ = new Simulator(DroneId, update, stop);
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
            {
                if (func(typeof(T).Name.Contains("ForList") ? item : GetObject(item.Id, item.GetType().Name)))
                    yield return item;
            }
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


        public void DeletePackage(int packageId)
        {
            if (GetDALPackage(packageId).DroneId is null)
                Idal.DeletePackage(packageId);
        }

        public IEnumerable<StationForList> GetAllStationsWhere(Func<StationForList, bool> func)
        {
            return GetObjectsWhere(func).Cast<StationForList>();
        }

        public IEnumerable<DroneForList> GetAllDronesWhere(Func<DroneForList, bool> func)
        {
            return GetObjectsWhere(func).Cast<DroneForList>();
        }

        public IEnumerable<CustomerForList> GetAllCustomersWhere(Func<CustomerForList, bool> func)
        {
            return GetObjectsWhere(func).Cast<CustomerForList>();
        }

        public IEnumerable<PackageForList> GetAllPackagesWhere(Func<PackageForList, bool> func)
        {
            return GetObjectsWhere(func).Cast<PackageForList>();
        }
    }
}
