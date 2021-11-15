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


        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots)
        {
            Idal.AddStation(id, name, longitude, lattitude, chargeSlots);
            
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
            Idal.
            throw new NotImplementedException();
        }

        public void UpdateStation(int id, string newName = "", int newChargeSlots = -1)
        {
            Idal.
            throw new NotImplementedException();
        }

        public void UpdateCustomer(int id, string newName = "", int newPhone = -1)
        {
            throw new NotImplementedException();
        }

        public void SendDroneToCharge(int DroneId)
        {
            throw new NotImplementedException();
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
    }
}
