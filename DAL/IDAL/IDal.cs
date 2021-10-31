using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    interface IDal
    {
        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots);
        public void AddDrone(int id, string model, WeightGroup weight);
        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude);
        public void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive);
        public void GivePackageDrone(int packageId, int droneId);
        public void PickUpPackage(int packageId, int droneID);

        public void DeliverPackage(int packageId);
        public void SendDroneToCharge(int droneId, int stationId);
        public void ReleaseDroneFromCharge(int droneId, int stationId);

        public string GetStationString(int id);
        public string GetDroneString(int id);
        public string GetCustomerString(int id);
        public string GetPackageString(int id);


        public IEnumerable<Station> GetAllStations();
        public IEnumerable<Drone> GetAllDrones();
        public IEnumerable<Customer> GetAllCustomers();
        public IEnumerable<Package> GetAllPackages();
        public IEnumerable<Package> GetAllUndronedPackages();
        public IEnumerable<Station> GetAllAvailableStations();

        public IEnumerable<double> GetElectricity(int droneId);
    }
}
