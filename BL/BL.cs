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
            throw new NotImplementedException();
        }

        public void UpdateStation(int id, string newName = "", int newChargeSlots = -1)
        {
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

        public string DisplayStation(int StationId)
        {
            throw new NotImplementedException();
        }

        public string DisplayDrone(int DroneId)
        {
            throw new NotImplementedException();
        }

        public string DisplayCustomer(int CustomerId)
        {
            throw new NotImplementedException();
        }

        public string DisplayPackage(int PackageId)
        {
            throw new NotImplementedException();
        }

        public string DisplayStations()
        {
            throw new NotImplementedException();
        }

        public string DisplayDrones()
        {
            throw new NotImplementedException();
        }

        public string DisplayCustomers()
        {
            throw new NotImplementedException();
        }

        public string DisplayPackages()
        {
            throw new NotImplementedException();
        }

        public string DisplayPackagesWithoutDrone()
        {
            throw new NotImplementedException();
        }

        public string DisplayStationsWithCharges()
        {
            throw new NotImplementedException();
        }
    }
}
