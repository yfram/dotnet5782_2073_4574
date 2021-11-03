using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IDAL.DO;

namespace DalObject
{
    public partial class DalObject : IDAL.IDal
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        #region Add functions
        //public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots) =>
         //   DataSource.Stations.Add(new(id, name, longitude, lattitude, chargeSlots));
         /*
        public void AddDrone(int id, string model, WeightGroup weight) =>
            DataSource.Drones.Add(new(id, model, weight));
         */
         /*
        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude) =>
            DataSource.Customers.Add(new(id, name, phone, lattitude, longitude));
         */
        /*
        public void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive) =>
            DataSource.Packages.Add(new(id, senderId, recevirId, weight, packagePriority, droneId,
                timeToPackage, timeToGetDrone, timeToGetPackedge, timeToRecive));
        */
        #endregion

        #region Update functions
        public void GivePackageDrone(int packageId, int droneId)
        {
            //this function could be one line, but Package is a struct, so that would not work. I hate structs.
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.DroneId = droneId;
            DataSource.Packages[index] = tmp;
        }
        public void PickUpPackage(int packageId, int droneID)
        {
            GivePackageDrone(packageId, droneID);
            int index = GetDroneIndex(droneID);
            Drone tmp = DataSource.Drones[index];
            //tmp.State = DroneStates.Shipping;
            DataSource.Drones[index] = tmp;
        }

        /*
        public void DeliverPackage(int packageId)
        {
            int packageIndex = GetPackageIndex(packageId);
            int droneIndex = GetDroneIndex(DataSource.Packages[packageIndex].DroneId.Value);
            Drone tmp = DataSource.Drones[droneIndex];
            //tmp.State = DroneStates.Empty;
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Packages.RemoveAt(packageIndex);

            //DataSource.Customers.RemoveAll(c => c.Id == package.RecevirId); was not sure this is needed, we might want to save a list of past customers
        }
        */
        public void SendDroneToCharge(int droneId, int stationId)
        {
            int droneIndex = GetDroneIndex(droneId);
            Drone tmp = DataSource.Drones[droneIndex];
            //tmp.State = DroneStates.Maintenance;//I think its Maintenance, should be at least
            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];
            tmp1.ChargeSlots--;
            DataSource.DroneCharges.Add(new(droneId, stationId));
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Stations[stationIndex] = tmp1;
        }
        public void ReleaseDroneFromCharge(int droneId, int stationId)
        {
            int droneIndex = GetDroneIndex(droneId);
            Drone tmp = DataSource.Drones[droneIndex];
            //tmp.State = DroneStates.Empty;//I think its Maintenance, should be at least
            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];
            tmp1.ChargeSlots++;
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Stations[stationIndex] = tmp1;
            DataSource.DroneCharges.RemoveAll(d => d.DroneId == droneId && d.StationId == stationId);
        }
        #endregion

        #region Get by Id Functions
        //public string GetStationString(int id) => DataSource.Stations[GetStationIndex(id)].ToString();
        //public string GetDroneString(int id) => DataSource.Drones[GetDroneIndex(id)].ToString();
        //public string GetCustomerString(int id) => DataSource.Customers[GetCustomerIndex(id)].ToString();
        //public string GetPackageString(int id) => DataSource.Packages[GetPackageIndex(id)].ToString();
        #endregion

        #region Get all IDAL.DO object Functions
        //public IEnumerable<Station> GetAllStations() => DataSource.Stations;
        //public IEnumerable<Drone> GetAllDrones() => DataSource.Drones;
        //public IEnumerable<Customer> GetAllCustomers() => DataSource.Customers;
        //public IEnumerable<Package> GetAllPackages() => DataSource.Packages;
        //public IEnumerable<Package> GetAllUndronedPackages() => DataSource.Packages.Where(p => p.DroneId == null);
        //public IEnumerable<Station> GetAllAvailableStations() => DataSource.Stations.Where(p => p.ChargeSlots > 0);




        #endregion

        private int GetDroneIndex(int id) => DataSource.Drones.FindIndex(d => d.Id == id);
        //private int GetStationIndex(int id) => DataSource.Stations.FindIndex(s => s.Id == id);
        private int GetCustomerIndex(int id) => DataSource.Customers.FindIndex(c => c.Id == id);
        //private int GetPackageIndex(int id) => DataSource.Packages.FindIndex(p => p.Id == id);


        public IEnumerable<double> GetElectricity(int droneId)
        {
            throw new NotImplementedException();
        }
    }
}
