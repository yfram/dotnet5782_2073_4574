﻿using System;
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
        private List<DroneForList> BLdrones;
        private IDal Idal;
        public BL()
        {
            Idal = new DalObject.DalObject();

            foreach(IDAL.DO.Drone DALdrone in Idal.GetAllDrones())
            {
                Drone BLdrone= new Drone();
                BLdrone.Id = DALdrone.Id;
                BLdrone.Model = DALdrone.Model;
                BLdrone.Weight = DALdrone.Weight;

                //TODO: BLdrone.Package

                IEnumerable<IDAL.DO.Package> AccosiatedButNotDelivered = Idal.GetAllPackages().Where(p => p.DroneId.HasValue && p.DroneId == DALdrone.Id&&(p.Delivered == DateTime.MinValue)).();
                
                if (AccosiatedButNotDelivered.Count() != 0) // there are packages that accosiated but not delivered
                {
                    BLdrone.State = DroneState.Bussy;
                    IDAL.DO.Package p = AccosiatedButNotDelivered.First();

                    bool WasPickUp = (p.PickUp != DateTime.MinValue);

                    if(WasPickUp) // the location of the sender
                    {
                        IDAL.DO.Customer customer = Idal.GetCustomer(p.SenderId); // TODO: TRY CATCH
                        BLdrone.CurrentLocation = new Location(customer.Longitude, customer.Lattitude);
                    }
                    else // the closet station to the sender
                    {

                    }
                    // add batery


                } // and if AccosiatedButNotDelivered

                else
                {
                    int state = new Random().Next(0, 2);
                    if(state == 0) // Maitenance
                    {
                        BLdrone.State = DroneState.Maitenance;

                        // batery between 0 to 20
                        BLdrone.Battery = ((double)new Random().Next(0,21))/100;

                        // location in one of the stations
                        var stations = Idal.GetAllStations();
                        var station = stations.ElementAt(new Random().Next(0,stations.Count()));
                        BLdrone.CurrentLocation = new Location(station.Longitude, station.Lattitude);
                           
                    }
                    else //empty
                    {
                        // מיקומו יוגרל בין לקוחות שיש חבילות שסופקו להם
                        
                    }
                }

            }

        }


        public void AddStation(Station s)
        {
            Idal.AddStation(s.Id, s.Name, s.LocationOfStation.Longitude, s.LocationOfStation.Latitude, s.ChargingDrones.Count + s.AmountOfEmptyPorts); ;
        }

        public void AddDrone(int id, string model, WeightGroup weight, int stationId)
        {
            Idal.AddDrone(id, model, weight);
            Idal.SendDroneToCharge(id, stationId);

            double battery = 0;//TODO;

            BLdrones.Add(new DroneForList(id,model,weight, battery,DroneState.Maitenance,));

        }

        public void AddCustomer(int id, string name, string phone, Location loc)
        {
            throw new NotImplementedException();
        }

        public void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, PriorityGroup packagePriority)
        {
            throw new NotImplementedException();
        }

        public void UpdateDroneName(int id, string newModel)
        {
            IDAL.DO.Drone DALdrone = Idal.GetDrone(id);
            DALdrone.Model = newModel;

            Idal.UpdateDrone(DALdrone);

            IEnumerable<DroneForList> myDrone = BLdrones.Where(d => d.Id == id);
            if(myDrone.Count() == 1)
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
            DroneForList drone = BLdrones.Find(d=>d.Id == DroneId);
            if(drone != null && drone.State == DroneState.Empty)
            {
                int? ClosestId = GetClosetStation(drone.CurrentLocation);
                if (ClosestId != null)
                {
                    IDAL.DO.Station closest = Idal.GetStation((int)ClosestId);
                    double amoutOfBattery = DroneGoNewBattery(drone, DistanceTo(new Location(closest.Longitude, closest.Lattitude),drone.CurrentLocation));
                    if(amoutOfBattery >= 0)
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

            if(BLdrone != null)
            {
                if(BLdrone.State == DroneState.Maitenance)
                {
                    BLdrone.Battery += Idal.GetElectricity()[4]*time;
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
            if(BLdrone != null)
            {
                if(BLdrone.State == DroneState.Empty)
                {
                    IEnumerable<IDAL.DO.Package> allPackages = Idal.GetAllUndronedPackages();

                    IEnumerable<IDAL.DO.Package> allCanWeight = allPackages.Where(p => (int)p.Weight <= (int)BLdrone.Weight);

                    if (allCanWeight.Count() > 0)
                    {
                        IEnumerable<IDAL.DO.Package> allNear = allCanWeight.Where(p => DroneHaveEnoughBattery(p, BLdrone));

                        List<IDAL.DO.Package> allNearList = allNear.ToList();
                        allNearList.Sort((p1, p2) => PackagePriority(p1, p2, BLdrone.CurrentLocation));

                        int PackageId = allNearList[0].Id;

                        BLdrone.State = DroneState.Bussy;
                        BLdrone.PassingPckageId = PackageId;

                        Idal.GivePackageDrone(PackageId,BLdrone.Id); // change drone id in package, change time assosiating
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
        private int PackagePriority(IDAL.DO.Package p1 , IDAL.DO.Package p2, Location DroneLoc)
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

            Location LocP1 = new Location(p1Cust.Longitude,p1Cust.Lattitude);
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
         * Enough battery to go the sender, then go from him to recivier and return back to the station
         */
        private bool DroneHaveEnoughBattery(IDAL.DO.Package p,DroneForList d)
        {
            double maxDistance = DroneMaxDistance(d);

            var sender = Idal.GetCustomer(p.SenderId);
            Location senderLoc = new Location(sender.Longitude, sender.Lattitude);

            var recv = Idal.GetCustomer(p.RecevirId);
            Location recvLoc = new Location(recv.Longitude, recv.Lattitude);

            int? StationId = GetClosetStation(recvLoc);
            if(StationId == null)
            {
                /**
             * ATTENTION: now,if all the stations have no charging slot, the associationg cannot be!!! 
             */
                return false;
            }
            IDAL.DO.Station closest = Idal.GetStation((int)StationId);
            Location closestLoc = new Location(closest.Longitude, closest.Lattitude)
                ;
            double distance = DistanceTo(d.CurrentLocation, senderLoc) + DistanceTo(recvLoc,senderLoc) + DistanceTo(closestLoc, recvLoc);

            return distance <= maxDistance;
        }
        public void PickPackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId); // replace it with get by id

            if(BLdrone.State == DroneState.Bussy)
            {
                int PackageId = BLdrone.Id;
                IDAL.DO.Package p = Idal.GetPackage(PackageId);
                if(p.Associated != DateTime.MinValue && p.PickUp == DateTime.MinValue)
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

        public void GivePackage(int DroneId)
        {
            DroneForList BLdrone = BLdrones.First(d => d.Id == DroneId); // replace it with get by id

            if (BLdrone.State == DroneState.Bussy)
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

        public IEnumerable<StationForList> DisplayStation(int StationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DroneForList> DisplayDrone(int DroneId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CustomerForList> DisplayCustomer(int CustomerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<PackageForList> DisplayPackage(int PackageId)
        {
            throw new NotImplementedException();
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

        private static double DistanceTo(Location Loc1, Location Loc2,char unit = 'K')
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

        private int? GetClosetStation(Location loc)
        {
            IEnumerable<IDAL.DO.Station> FreeStations = Idal.GetAllStations().Where(s => s.ChargeSlots > 0);
            if (FreeStations.Count() > 0)
            {
                IDAL.DO.Station closest = FreeStations.Aggregate((s1, s2) => DistanceTo(new Location(s1.Longitude, s1.Lattitude), drone.CurrentLocation)
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
            double[] elec = Idal.GetElectricity();
            int ix = -1;
            if (d.State == DroneState.Empty)
                ix = 0;
            else if (d.State == DroneState.Bussy)
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
