using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL.BO;
using IDAL;

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
                    Where(p => p.DroneId.HasValue &&
                    (p.Delivered == DateTime.MinValue)).FirstOrDefault();

                if (AssociatedButNotDelivered as object is not null)
                {
                    BLdrone.State = DroneState.Busy;
                    BLdrone.PassingPckageId = AssociatedButNotDelivered.Id;
                    bool WasPickUp = (AssociatedButNotDelivered.PickUp != DateTime.MinValue);
                    //no need here for a try block, as we know that the senderId exists
                    IDAL.DO.Customer customer = Idal.GetCustomer(AssociatedButNotDelivered.SenderId);
                    Location customerLoc = new Location(customer.Longitude, customer.Lattitude);
                    if (WasPickUp)
                    {
                        BLdrone.CurrentLocation = customerLoc;
                    }
                    else // accosiatied but not picked. the closet station to the sender
                    {
                        int? stationId = GetClosetStation(new Location(customer.Longitude, customer.Lattitude), false);
                        if (stationId != null)
                        {
                            IDAL.DO.Station s = Idal.GetStation((int)stationId);
                            BLdrone.CurrentLocation = new Location(s.Longitude, s.Lattitude);
                        }
                        else
                        {
                            throw new NoFreeStationsAvailable(DALdrone.Id, new Location(customer.Longitude, customer.Lattitude));
                        }
                    }
                    // add batery
                    double distance = DistanceToDoDeliver(AssociatedButNotDelivered, BLdrone);
                    double minBattery = distance / ElecOfDrone(BLdrone);
                    if (minBattery > 1)
                        throw new TooFarException("There is not enough battery to get there!");
                    BLdrone.Battery = rand.NextDouble() * (1 - minBattery) + minBattery;
                }

                else
                {
                    int state = rand.Next(0, 2);
                    if (state == 0) // Maitenance
                    {
                        BLdrone.State = DroneState.Maitenance;

                        // batery between 0 to 20
                        BLdrone.Battery = ((double)rand.Next(0, 21)) / 100;

                        // location in one of the stations
                        var stations = Idal.GetAllStations();
                        var station = stations.ElementAt(rand.Next(0, stations.Count()));
                        BLdrone.CurrentLocation = new Location(station.Longitude, station.Lattitude);

                    }
                    else //empty
                    {
                        BLdrone.State = DroneState.Empty;
                        // מיקומו יוגרל בין לקוחות שיש חבילות שסופקו להם

                        var empyPackages = Idal.GetAllUndronedPackages();
                        if (empyPackages.Count() == 0)
                        {
                            // what should I do?
                        }
                        else
                        {
                            var sender = Idal.GetCustomer(empyPackages.First().SenderId);
                            Location locPac = new Location(sender.Longitude, sender.Lattitude);
                            BLdrone.CurrentLocation = locPac;
                            int? stationId = GetClosetStation(locPac);
                            if (stationId == null)
                            {
                                // throw
                            }
                            var station = Idal.GetStation((int)stationId);
                            var stationLoc = new Location(station.Longitude, station.Lattitude);
                            double minBattery = DistanceTo(stationLoc, locPac) / ElecOfDrone(BLdrone);
                            if (minBattery > 1)
                            {
                                //throw
                            }
                            BLdrone.Battery = rand.NextDouble() * (1 - minBattery) + minBattery;

                        }

                    }
                }
                BLdrones.Add(BLdrone);
            }

        }


        public void AddStation(Station s)
        {
            Idal.AddStation(s.Id, s.Name, s.LocationOfStation.Longitude, s.LocationOfStation.Latitude, s.ChargingDrones.Count + s.AmountOfEmptyPorts); ;
        }

        public void AddDrone(Drone d)
        {
            Idal.AddDrone(d.Id, d.Model, (IDAL.DO.WeightGroup)((int)d.Weight));

            DroneForList df = new DroneForList(d.Id, d.Model, d.Weight, d.Battery, d.State, d.CurrentLocation);
            BLdrones.Add(df);

        }

        public void AddCustomer(Customer c)
        {
            Idal.AddCustomer(c.Id, c.Name, c.PhoneNumber, c.CustomerLocation.Latitude, c.CustomerLocation.Longitude);
        }

        public void AddPackage(Package p)
        {
            Idal.AddPackage(p.Id, p.Sender.Id, p.Reciver.Id, (IDAL.DO.WeightGroup)((int)p.Weight), ((IDAL.DO.Priority)(int)p.Priority), null, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
        }

        public void UpdateDroneName(int id, string newModel)
        {
            IDAL.DO.Drone DALdrone = Idal.GetDrone(id);
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
            IDAL.DO.Station station = Idal.GetStation(id);
            station.Name = newName != "" ? newName : station.Name;
            station.ChargeSlots = newChargeSlots > 0 ? newChargeSlots : station.ChargeSlots;
            Idal.UpdateStation(station);
        }

        public void UpdateCustomer(int id, string newName = "", string newPhone = "")
        {
            IDAL.DO.Customer customer = Idal.GetCustomer(id);
            customer.Name = newName != "" ? newName : customer.Name;
            customer.Phone = newPhone != "" ? newPhone : customer.Phone;
            Idal.UpdateCustomer(customer);
        }

        public void SendDroneToCharge(int DroneId)
        {
            DroneForList drone = BLdrones.Find(d => d.Id == DroneId);
            if (drone != null && drone.State == DroneState.Empty)
            {
                int? ClosestId = GetClosetStation(drone.CurrentLocation);
                if (ClosestId != null)
                {
                    IDAL.DO.Station closest = Idal.GetStation((int)ClosestId);
                    double amoutOfBattery = DroneGoNewBattery(drone, DistanceTo(new Location(closest.Longitude, closest.Lattitude), drone.CurrentLocation));
                    if (amoutOfBattery >= 0)
                    {
                        // we can send the drone for charging!!
                        drone.Battery -= amoutOfBattery;
                        drone.CurrentLocation = new Location(closest.Longitude, closest.Lattitude);
                        drone.State = DroneState.Maitenance;

                        Idal.SendDroneToCharge(drone.Id, closest.Id);

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
            else
            {
                //throw
            }



        }

        public void ReleaseDrone(int DroneId, double time)
        {
            IDAL.DO.Drone DALdrone = Idal.GetDrone(DroneId);
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);

            if (BLdrone != null)
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
                    //throw
                }
            }
            else
            {
                //throw
            }

        }

        public void AssignPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.Find(d => d.Id == DroneId);
            if (BLdrone != null)
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
                        //throw
                    }



                }
                else
                {
                    // throw
                }
            }
            else
            {
                // throw.
            }
        }

        /**
         * 0 if eq 
         * -1 if want second
         * 1 if want first
        */
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

            Location LocP1 = new Location(p1Cust.Longitude, p1Cust.Lattitude);
            Location LocP2 = new Location(p2Cust.Longitude, p2Cust.Lattitude);

            double p1Dist = DistanceTo(DroneLoc, LocP1);
            double p2Dist = DistanceTo(DroneLoc, LocP2);

            if (p1Dist > p2Dist)
                return -1;
            if (p1Dist < p2Dist)
                return 1;

            return 0;
        }


        /*
         * 
         * 
         */
        private double DistanceToDoDeliver(IDAL.DO.Package p, DroneForList d)
        {
            var sender = Idal.GetCustomer(p.SenderId);
            Location senderLoc = new Location(sender.Longitude, sender.Lattitude);

            var recv = Idal.GetCustomer(p.RecevirId);
            Location recvLoc = new Location(recv.Longitude, recv.Lattitude);

            int? StationId = GetClosetStation(recvLoc);
            if (StationId == null)
            {
                /**
             * ATTENTION: now,if all the stations have no charging slot, the associationg cannot be!!! 
             */
                return double.PositiveInfinity;
            }
            IDAL.DO.Station closest = Idal.GetStation((int)StationId);
            Location closestLoc = new Location(closest.Longitude, closest.Lattitude)
                ;
            double distance = DistanceTo(d.CurrentLocation, senderLoc) + DistanceTo(recvLoc, senderLoc) + DistanceTo(closestLoc, recvLoc);

            return distance;
        }


        /*
         * Enough battery to go the sender, then go from him to recivier and return back to the station
         */
        private bool DroneHaveEnoughBattery(IDAL.DO.Package p, DroneForList d)
        {
            double maxDistance = DroneMaxDistance(d);
            double distance = DistanceToDoDeliver(p, d);
            return distance <= maxDistance;
        }

        public void PickUpPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId); // replace it with get by id

            if (BLdrone.State == DroneState.Busy)
            {
                int PackageId = BLdrone.Id;
                IDAL.DO.Package p = Idal.GetPackage(PackageId);
                if (p.Associated != DateTime.MinValue && p.PickUp == DateTime.MinValue)
                {
                    // the battery was checked in associate

                    IDAL.DO.Customer Sender = Idal.GetCustomer(p.SenderId);
                    Location SenderLoc = new Location(Sender.Longitude, Sender.Lattitude);

                    BLdrone.Battery -= ElecOfDrone(BLdrone) * DistanceTo(BLdrone.CurrentLocation, SenderLoc);
                    BLdrone.CurrentLocation = SenderLoc;
                    Idal.PickUpPackage(PackageId, BLdrone.Id); // update time
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
                    Location RecvLoc = new Location(Recv.Longitude, Recv.Lattitude);

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

        private IDAL.DO.Station GetDALStation(int StationId)
        {
            return Idal.GetStation(StationId);
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

            return new Station(DALstation.Id, DALstation.Name, new Location(DALstation.Longitude, DALstation.Lattitude), DALstation.ChargeSlots, charged);
        }

        private IDAL.DO.Drone GetDALDrone(int DroneId)
        {
            return Idal.GetDrone(DroneId); // Do exceptions and things
        }

        private IDAL.DO.Package GetDALPackage(int PackageId)
        {
            return Idal.GetPackage(PackageId); // Do exceptions and things
        }

        private IDAL.DO.Customer GetDALCustomer(int CustomerId)
        {
            return Idal.GetCustomer(CustomerId); // Do exceptions and things
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

                Location locSend = new Location(DALsend.Longitude, DALsend.Lattitude), locRecv = new Location(DALrecv.Longitude, DALrecv.Lattitude);
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

            return new Customer(DALcustomer.Id, DALcustomer.Name, DALcustomer.Name, new Location(DALcustomer.Longitude, DALcustomer.Lattitude), pkgFrom, pkgTo);
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
            throw new NotImplementedException();
        }

        public IEnumerable<DroneForList> DisplayDrones()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerForList> DisplayCustomers()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageForList> DisplayPackages()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageForList> DisplayPackagesWithoutDrone()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StationForList> DisplayStationsWithCharges()
        {
            throw new NotImplementedException();
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
                IDAL.DO.Station closest = FreeStations.Aggregate((s1, s2) => DistanceTo(new Location(s1.Longitude, s1.Lattitude), loc)
               > DistanceTo(new Location(s2.Longitude, s2.Lattitude), loc)
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
                return -1;
            return elec[ix];
        }
        private bool DroneCanGO(DroneForList d, double distance)
        {
            return DroneMaxDistance(d) <= distance;
        }

    }
}
