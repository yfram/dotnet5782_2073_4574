using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IDal
    {
        void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots);
        void AddDrone(int id, string model, WeightGroup weight);
        void AddCustomer(int id, string name, string phone, double lattitude, double longitude);
        void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            DateTime Created, DateTime Associated, DateTime PickUp, DateTime Delivered);
        void GivePackageDrone(int packageId, int droneId);
        void PickUpPackage(int packageId, int droneID);

        void DeliverPackage(int packageId);
        void SendDroneToCharge(int droneId, int stationId);
        void ReleaseDroneFromCharge(int droneId, int stationId);

        string GetStationString(int id);
        string GetDroneString(int id);
        Customer GetCustomer(int id);
        string GetPackageString(int id);


        IEnumerable<Station> GetAllStations();
        IEnumerable<Drone> GetAllDrones();
        IEnumerable<Customer> GetAllCustomers();
        IEnumerable<Package> GetAllPackages();
        IEnumerable<Package> GetAllUndronedPackages();
        IEnumerable<Station> GetAllAvailableStations();

        IEnumerable<double> GetElectricity(int droneId);
    }
}
