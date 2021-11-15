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
                IEnumerable<IDAL.DO.Station> FreeStations =  Idal.GetAllStations().Where(s => s.ChargeSlots > 0);
                if (FreeStations.Count() > 0)
                {
                    IDAL.DO.Station closest = FreeStations.Aggregate((s1, s2) => DistanceTo(new Location(s1.Longitude, s1.Lattitude), drone.CurrentLocation)
                   > DistanceTo(new Location(s2.Longitude, s2.Lattitude), drone.CurrentLocation)
                   ? s2 : s1);

                    
                    double amoutOfBattery = DroneGoNewBattery(drone, DistanceTo(new Location(closest.Longitude, closest.Lattitude),drone.CurrentLocation));
                    if(amoutOfBattery >= 0)
                    {
                        // we can send the drone for charging!!
                        drone.Battery -= amoutOfBattery;
                        drone.CurrentLocation = new Location(closest.Longitude, closest.Lattitude);
                        drone.State = DroneState.Maitenance;

                        UpdateStation(closest.Id,"" ,closest.ChargeSlots - 1);

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
            throw new NotImplementedException();
        }

        public void AssignPackage(int DroneId)
        {
            throw new NotImplementedException();
        }

        public void PickPackage(int DroneId)
        {
            throw new NotImplementedException();
        }

        public void GivePackage(int DroneId)
        {
            throw new NotImplementedException();
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

            return d.Battery - elec[ix] * distance;
        }
        private bool DroneCanGO(DroneForList d, double distance)
        {
            return DroneGoNewBattery(d, distance) >= 0;
        }

    }
}
