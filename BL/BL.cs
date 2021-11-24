﻿using IBL.BO;
using IBL.Exceptions;
using IDAL;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IBL
{
    public partial class BL : IBL
    {
        private List<DroneForList> BLdrones = new();
        private IDal Idal = new DalObject.DalObject();
        private double[] elecRate;
        private static Random rand = new();


        public BL()
        {
            elecRate = Idal.GetElectricity();
            foreach (IDAL.DO.Drone DALdrone in Idal.GetAllDrones())
            {
                DroneForList BLdrone = new();
                BLdrone.Id = DALdrone.Id;
                BLdrone.Model = DALdrone.Model;
                BLdrone.Weight = (WeightGroup)((int)DALdrone.Weight);

                IDAL.DO.Package AssociatedButNotDelivered = Idal.GetAllPackages().
                    Where(p => p.DroneId == (int?)BLdrone.Id &&
                    (p.Delivered == DateTime.MinValue)).FirstOrDefault();

                if (AssociatedButNotDelivered as object is not null)
                {
                    BLdrone.State = DroneState.Busy;
                    BLdrone.PassingPckageId = AssociatedButNotDelivered.Id;

                    bool WasPickedUp = AssociatedButNotDelivered.PickUp != DateTime.MinValue;
                    //no need here for a try block, as we know that the senderId exists
                    IDAL.DO.Customer customer = Idal.GetCustomer(AssociatedButNotDelivered.SenderId);
                    Location customerLoc = new(customer.Longitude, customer.Lattitude);
                    if (WasPickedUp)
                    {
                        BLdrone.CurrentLocation = customerLoc;
                    }
                    else // accosiatied but not picked. the closet station to the sender
                    {
                        int? stationId = GetClosetStation(new(customer.Longitude, customer.Lattitude), false);
                        IDAL.DO.Station s = Idal.GetStation((int)stationId);
                        BLdrone.CurrentLocation = new(s.Longitude, s.Lattitude);
                    }
                    // add batery
                    double distance = DistanceToDoDeliver(AssociatedButNotDelivered, BLdrone);
                    double minBattery = distance / ElecOfDrone(BLdrone);
                    if (minBattery > 100)//the minumun battery needed for the delivery is larger than 100% charge
                        throw new BlException("Not enough free stations!", BLdrone.Id, typeof(Drone));
                    BLdrone.Battery = rand.NextDouble() * (1 - minBattery) + minBattery;
                }

                else
                {
                    int state = rand.Next(0, 2);
                    if (state == 0) // Maitenance
                    {
                        BLdrone.State = DroneState.Maitenance;

                        BLdrone.Battery = ((double)rand.Next(0, 21)) / 100;

                        var stations = Idal.GetAllStations();
                        var station = stations.ElementAt(rand.Next(0, stations.Count()));
                        BLdrone.CurrentLocation = new(station.Longitude, station.Lattitude);

                    }
                    else //empty
                    {
                        BLdrone.State = DroneState.Empty;
                        var allPackages = Idal.GetAllPackages().Where(p => p.Delivered != DateTime.MinValue);
                        var customer = GetDALCustomer(allPackages.ToList()[rand.Next(0, allPackages.Count())].RecevirId);
                        BLdrone.CurrentLocation = new(customer.Longitude, customer.Lattitude);

                        int? id = GetClosetStation(BLdrone.CurrentLocation);
                        if (id is null) throw new BlException("No free charging stations", BLdrone.Id, typeof(Drone));

                        var station = GetDALStation((int)id);
                        Location stationLoc = new(station.Longitude, station.Lattitude);
                        double minBattery = DistanceTo(stationLoc, BLdrone.CurrentLocation) / ElecOfDrone(BLdrone);
                        if (minBattery > 100)
                        {
                            throw new BlException("Not enough free stations!", BLdrone.Id, typeof(Drone));
                        }
                        BLdrone.Battery = rand.NextDouble() * (1 - minBattery) + minBattery;
                    }
                }
                BLdrones.Add(BLdrone);
            }

        }

        public void AddStation(Station s)
        {
            try
            {
                Idal.AddStation(s.Id, s.Name, s.LocationOfStation.Longitude, s.LocationOfStation.Latitude, s.ChargingDrones.Count + s.AmountOfEmptyPorts); ;
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        public void AddDrone(Drone d)
        {
            try
            {
                Idal.AddDrone(d.Id, d.Model, (IDAL.DO.WeightGroup)((int)d.Weight));

                DroneForList df = new DroneForList(d.Id, d.Model, d.Weight, d.Battery, d.State, d.CurrentLocation);
                BLdrones.Add(df);
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

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

        public void AddPackage(Package p)
        {
            try
            {
                Idal.AddPackage(p.Id, p.Sender.Id, p.Reciver.Id, (IDAL.DO.WeightGroup)((int)p.Weight), ((IDAL.DO.Priority)(int)p.Priority), null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
            }
            catch (Exception e)
            {
                throw new ObjectAllreadyExistsException(e.Message);
            }
        }

        public void UpdateDroneName(int id, string newModel)
        {
            IDAL.DO.Drone DALdrone = GetDALDrone(id);
            DALdrone.Model = newModel;

            Idal.UpdateDrone(DALdrone);

            IEnumerable<DroneForList> myDrone = BLdrones.Where(d => d.Id == id);
            if (myDrone.Count() == 1)
            {
                DroneForList BLdrone = myDrone.ElementAt(0);
                BLdrone.Model = DALdrone.Model;
            }
        }

        public void UpdateStation(int id, string newName = "", int newChargeSlots = -1)
        {
            IDAL.DO.Station station = GetDALStation(id);
            Station s = DisplayStation(id);
            int totalPorts = s.AmountOfEmptyPorts + s.ChargingDrones.Count;


            station.Name = newName != "" ? newName : station.Name;

            if (newChargeSlots >= totalPorts)
                newChargeSlots -= s.ChargingDrones.Count;
            else if (newChargeSlots == -1)
                newChargeSlots = station.ChargeSlots;
            else
                throw new ArgumentException($"the new Chrage slots({newChargeSlots}) must be big than the number of charging drones right now(${s.ChargingDrones}).");

            station.ChargeSlots = newChargeSlots;
            Idal.UpdateStation(station);
        }

        public void UpdateCustomer(int id, string newName = "", string newPhone = "")
        {
            IDAL.DO.Customer customer = GetDALCustomer(id);
            customer.Name = newName != "" ? newName : customer.Name;
            customer.Phone = newPhone != "" ? newPhone : customer.Phone;
            Idal.UpdateCustomer(customer);
        }

        public void SendDroneToCharge(int DroneId)
        {
            DroneForList drone = BLdrones.Find(d => d.Id == DroneId);
            if (drone is not null && drone.State == DroneState.Empty)
            {
                int? ClosestId = GetClosetStation(drone.CurrentLocation);
                if (ClosestId is not null)
                {
                    IDAL.DO.Station closest = Idal.GetStation((int)ClosestId);
                    double amoutOfBattery = DroneGoNewBattery(drone, DistanceTo(new(closest.Longitude, closest.Lattitude), drone.CurrentLocation));
                    if (amoutOfBattery >= 0)
                    {
                        // we can send the drone for charging!!
                        drone.Battery -= amoutOfBattery;
                        drone.CurrentLocation = new(closest.Longitude, closest.Lattitude);
                        drone.State = DroneState.Maitenance;

                        Idal.SendDroneToCharge(drone.Id, closest.Id);

                    }
                    else
                    {
                        throw new BlException($"there are no emtpy stations that {DroneId} has enogth battery to fly them!");
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
                    throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
                throw new DroneStateException($"the drone {DroneId} can send to chage only if it empy!");
            }



        }

        public void ReleaseDrone(int DroneId, double time)
        {
            IDAL.DO.Drone DALdrone = Idal.GetDrone(DroneId);
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);

            if (BLdrone is not null)
            {
                if (BLdrone.State == DroneState.Maitenance)
                {
                    BLdrone.Battery += elecRate[4] * time;
                    BLdrone.Battery = BLdrone.Battery > 1 ? 1 : BLdrone.Battery;
                    BLdrone.State = DroneState.Empty;
                    Idal.ReleaseDroneFromCharge(DroneId, -1); // find the station id by yourelf, via the DroneCharges object

                }
                else
                {
                    throw new DroneStateException($"the drone {DroneId} can be release only if it state is in Maitenance!");
                }
            }
            else
            {
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
            }

        }

        public void AssignPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);
            if (BLdrone is not null)
            {
                if (BLdrone.State == DroneState.Empty)
                {
                    IEnumerable<IDAL.DO.Package> allPackages = Idal.GetAllUndronedPackages();

                    IEnumerable<IDAL.DO.Package> allCanWeight = allPackages.Where(p => (int)p.Weight <= (int)BLdrone.Weight);

                    if (allCanWeight.Count() > 0)
                    {
                        IEnumerable<IDAL.DO.Package> allNear = allCanWeight.Where(p => DroneHaveEnoughBattery(p, BLdrone));

                        List<IDAL.DO.Package> allNearList = allNear.ToList();
                        allNearList.Sort((p1, p2) => PackagePriority(p1, p2, BLdrone.CurrentLocation));

                        int PackageId = allNearList[0].Id;

                        BLdrone.State = DroneState.Busy;
                        BLdrone.PassingPckageId = PackageId;

                        Idal.GivePackageDrone(PackageId, BLdrone.Id); // change drone id in package, change time assosiating
                    }
                    else
                    {
                        throw new BlException($"there are no free package for the ", DroneId, typeof(Drone));
                    }



                }
                else
                {
                    throw new DroneStateException($"the drone {DroneId} cannot be associated because it is not empty!");
                }
            }
            else
            {
                throw new ObjectDoesntExistException($"the drone {DroneId} is not exsist!");
            }
        }

        public void PickUpPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId);

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.Id;
                IDAL.DO.Package p = Idal.GetPackage(PackageId);
                if (p.Associated != DateTime.MinValue && p.PickUp == DateTime.MinValue)
                {
                    // the battery was checked in associate

                    IDAL.DO.Customer Sender = Idal.GetCustomer(p.SenderId);
                    Location SenderLoc = new(Sender.Longitude, Sender.Lattitude);

                    BLdrone.Battery -= ElecOfDrone(BLdrone) * DistanceTo(BLdrone.CurrentLocation, SenderLoc);
                    BLdrone.CurrentLocation = SenderLoc;
                    Idal.PickUpPackage(PackageId, BLdrone.Id); // update time
                }
                else
                {
                    throw new Exception("the package is not associated or picked up");
                }

            }
            else
            {
                throw new DroneStateException($"cannot pick up with drone that is not busy! the current state is {BLdrone.State}");
            }

        }

        public void DeliverPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId); // replace it with get by id

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.Id;
                IDAL.DO.Package p = Idal.GetPackage(PackageId);
                if (p.Delivered != DateTime.MinValue && p.Delivered == DateTime.MinValue)
                {
                    // the battery was checked in associate

                    IDAL.DO.Customer Recv = Idal.GetCustomer(p.RecevirId);
                    Location RecvLoc = new(Recv.Longitude, Recv.Lattitude);

                    BLdrone.Battery -= ElecOfDrone(BLdrone) * DistanceTo(BLdrone.CurrentLocation, RecvLoc);
                    BLdrone.CurrentLocation = RecvLoc;
                    BLdrone.State = DroneState.Empty;

                    p.Delivered = DateTime.Now;
                    Idal.UpdatePackage(p);
                }
                else
                {
                    //throw
                }

            }
            else
            {
                //throw
            }
        }

        public Station DisplayStation(int StationId)
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

        public Drone DisplayDrone(int DroneId)
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
                pckTransfer = new PackageInTransfer(pkgId, DALpkg.Delivered != DateTime.MinValue, (WeightGroup)(int)DALpkg.Weight, (PriorityGroup)(int)DALpkg.PackagePriority, send, recv, locSend, locRecv, DistanceTo(locRecv, locSend));
            }

            return new Drone(droneForList.Id, droneForList.Model, droneForList.Weight, droneForList.Battery, droneForList.State, pckTransfer, droneForList.CurrentLocation);
        }

        public Customer DisplayCustomer(int CustomerId)
        {

            var DALcustomer = GetDALCustomer(CustomerId);

            List<PackageForCustomer> pkgFrom = new();
            List<PackageForCustomer> pkgTo = new();

            IEnumerable<IDAL.DO.Package> DALpackages = Idal.GetAllPackages();

            foreach (IDAL.DO.Package dp in DALpackages)
            {
                if (dp.Associated != DateTime.MinValue) // if the package is in the progress of delivering
                {

                    if (dp.SenderId == CustomerId || dp.RecevirId == CustomerId)
                    {
                        PackageStatus status = dp.Delivered != DateTime.MinValue ? PackageStatus.Accepted : (dp.PickUp != DateTime.MinValue ? PackageStatus.PickedUp : dp.Associated != DateTime.MinValue ? PackageStatus.Paired : PackageStatus.Initialized);

                        PackageForCustomer pkgForCustomer = new(dp.Id, (WeightGroup)((int)dp.Weight), (PriorityGroup)((int)dp.PackagePriority), status, new CustomerForPackage(CustomerId, DALcustomer.Name));


                        if (dp.SenderId == CustomerId)
                            pkgFrom.Add(pkgForCustomer);
                        else // the Customer is the reciver of this pakcage
                            pkgTo.Add(pkgForCustomer);

                    }
                }
            }

            return new Customer(DALcustomer.Id, DALcustomer.Name, DALcustomer.Phone, new(DALcustomer.Longitude, DALcustomer.Lattitude), pkgFrom, pkgTo);
        }

        public Package DisplayPackage(int PackageId)
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

        public IEnumerable<StationForList> DisplayStations()
        {
            List<StationForList> ret = new();
            foreach (var station in Idal.GetAllStations())
            {
                Station blStation = DisplayStation(station.Id);
                ret.Add(new StationForList(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count));
            }
            return ret;
        }

        public IEnumerable<DroneForList> DisplayDrones()
        {
            List<DroneForList> ret = new();
            foreach (var drone in Idal.GetAllDrones())
            {
                Drone blDrone = DisplayDrone(drone.Id);
                ret.Add(new(blDrone.Id, blDrone.Model, blDrone.Weight, blDrone.Battery, blDrone.State, blDrone.CurrentLocation));
            }
            return ret;
        }

        public IEnumerable<CustomerForList> DisplayCustomers()
        {
            List<CustomerForList> ret = new();
            foreach (var customer in Idal.GetAllCustomers())
            {
                Customer blCustomer = DisplayCustomer(customer.Id);
                int sentAccepted = Idal.GetAllPackages().Where(p => p.SenderId == blCustomer.Id &&
                p.Delivered == DateTime.MinValue).Count();
                int sentOnTheWay = Idal.GetAllPackages().Where(p => p.SenderId == blCustomer.Id &&
                p.Delivered != DateTime.MinValue).Count();
                int accepted = Idal.GetAllPackages().Where(p => p.RecevirId == blCustomer.Id &&
                p.Delivered == DateTime.MinValue).Count();
                int onTheWay = Idal.GetAllPackages().Where(p => p.RecevirId == blCustomer.Id &&
                p.Delivered != DateTime.MinValue).Count();
                ret.Add(new CustomerForList(blCustomer.Id, blCustomer.Name, blCustomer.PhoneNumber, sentAccepted,
                    sentOnTheWay, accepted, onTheWay));
            }
            return ret;
        }

        public IEnumerable<PackageForList> DisplayPackages()
        {
            List<PackageForList> ret = new();
            foreach (var package in Idal.GetAllPackages())
            {
                Package blPackage = DisplayPackage(package.Id);
                ret.Add(new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority,
                    package.Delivered != DateTime.MinValue ?
                    PackageStatus.Accepted :
                    (package.PickUp != DateTime.MinValue ?
                    PackageStatus.PickedUp :
                    (package.Associated != DateTime.MinValue ?
                    PackageStatus.Paired :
                    PackageStatus.Initialized))));
            }
            return ret;
        }

        public IEnumerable<PackageForList> DisplayPackagesWithoutDrone()
        {
            List<PackageForList> ret = new();
            foreach (var package in Idal.GetAllUndronedPackages())
            {
                Package blPackage = DisplayPackage(package.Id);
                ret.Add(new(blPackage.Id, blPackage.Sender.Name, blPackage.Reciver.Name, blPackage.Weight, blPackage.Priority,
                    package.Delivered != DateTime.MinValue ?
                    PackageStatus.Accepted :
                    (package.PickUp != DateTime.MinValue ?
                    PackageStatus.PickedUp :
                    (package.Associated != DateTime.MinValue ?
                    PackageStatus.Paired :
                    PackageStatus.Initialized))));
            }
            return ret;
        }

        public IEnumerable<StationForList> DisplayStationsWithCharges()
        {
            List<StationForList> ret = new();
            foreach (var station in Idal.GetAllAvailableStations())
            {
                Station blStation = DisplayStation(station.Id);
                ret.Add(new(blStation.Id, blStation.Name, blStation.AmountOfEmptyPorts, blStation.ChargingDrones.Count));
            }
            return ret;
        }


        private IDAL.DO.Station GetDALStation(int StationId)
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

        private IDAL.DO.Drone GetDALDrone(int DroneId)
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

        private IDAL.DO.Package GetDALPackage(int PackageId)
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

        private IDAL.DO.Customer GetDALCustomer(int CustomerId)
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

        #region Utility
        private int PackagePriority(IDAL.DO.Package p1, IDAL.DO.Package p2, Location DroneLoc)
        {
            if ((int)p1.PackagePriority > (int)p2.PackagePriority)
                return 1;

            if ((int)p1.PackagePriority < (int)p2.PackagePriority)
                return -1;

            if ((int)p1.Weight > (int)p2.Weight)
                return 1;

            if ((int)p1.Weight < (int)p2.Weight)
                return -1;

            IDAL.DO.Customer p1Cust = Idal.GetCustomer(p1.SenderId);
            IDAL.DO.Customer p2Cust = Idal.GetCustomer(p2.SenderId);

            Location LocP1 = new(p1Cust.Longitude, p1Cust.Lattitude);
            Location LocP2 = new(p2Cust.Longitude, p2Cust.Lattitude);

            double p1Dist = DistanceTo(DroneLoc, LocP1);
            double p2Dist = DistanceTo(DroneLoc, LocP2);

            if (p1Dist > p2Dist)
                return -1;
            if (p1Dist < p2Dist)
                return 1;

            return 0;
        }

        private double DistanceToDoDeliver(IDAL.DO.Package p, DroneForList d)
        {
            var sender = Idal.GetCustomer(p.SenderId);
            Location senderLoc = new(sender.Longitude, sender.Lattitude);

            var recv = Idal.GetCustomer(p.RecevirId);
            Location recvLoc = new(recv.Longitude, recv.Lattitude);

            int? StationId = GetClosetStation(recvLoc);
            if (StationId == null)
            {
                /**
             * ATTENTION: now,if all the stations have no charging slot, the associationg cannot be!!! 
             */
                return double.PositiveInfinity;
            }
            IDAL.DO.Station closest = Idal.GetStation((int)StationId);
            Location closestLoc = new(closest.Longitude, closest.Lattitude)
                ;
            double distance = DistanceTo(d.CurrentLocation, senderLoc) + DistanceTo(recvLoc, senderLoc) + DistanceTo(closestLoc, recvLoc);

            return distance;
        }

        private bool DroneHaveEnoughBattery(IDAL.DO.Package p, DroneForList d)
        {
            double maxDistance = DroneMaxDistance(d);
            double distance = DistanceToDoDeliver(p, d);
            return distance <= maxDistance;
        }

        private static double DistanceTo(Location Loc1, Location Loc2, char unit = 'K')
        {
            double lat1 = Loc1.Latitude; double lon1 = Loc1.Longitude;
            double lat2 = Loc2.Latitude; double lon2 = Loc2.Longitude;

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

        private double DroneGoNewBattery(DroneForList d, double distance)
        {

            return d.Battery - ElecOfDrone(d) * distance;
        }

        private int? GetClosetStation(Location loc, bool toCharge = true)
        {
            IEnumerable<IDAL.DO.Station> FreeStations = Idal.GetAllStations();
            if (toCharge)
                FreeStations = FreeStations.Where(s => s.ChargeSlots > 0);
            if (FreeStations.Count() > 0)
            {
                IDAL.DO.Station closest = FreeStations.Aggregate((s1, s2) => DistanceTo(new(s1.Longitude, s1.Lattitude), loc)
               > DistanceTo(new(s2.Longitude, s2.Lattitude), loc)
               ? s2 : s1);

                return closest.Id;
            }
            else
            {
                return null;
            }
        }

        private double DroneMaxDistance(DroneForList d) => ElecOfDrone(d) * d.Battery;

        private double ElecOfDrone(DroneForList d)
        {
            double[] elec = this.elecRate;
            int ix = -1;
            if (d.State == DroneState.Empty)
                ix = 0;
            else if (d.State == DroneState.Busy)
            {
                switch (Idal.GetPackage((int)d.PassingPckageId).Weight)
                {
                    case IDAL.DO.WeightGroup.Light:
                        ix = 1;
                        break;
                    case IDAL.DO.WeightGroup.Mid:
                        ix = 2;
                        break;
                    case IDAL.DO.WeightGroup.Heavy:
                        ix = 3;
                        break;
                }
            }
            else // if the drone in Maitenance it can't go anyway
                throw new Exception("no drone weight!");
            return elec[ix];
        }
        #endregion
    }
}
