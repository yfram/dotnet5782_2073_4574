using DO;
using System;
using System.Collections.Generic;

namespace DalApi
{
    public interface IDAL
    {
        void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots);
        void AddDrone(int id, string model, WeightGroup weight);
        void AddCustomer(int id, string name, string phone, double lattitude, double longitude);
        void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId,
            DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered);
        void GivePackageDrone(int packageId, int droneId);
        void PickUpPackage(int packageId);

        void DeliverPackage(int packageId);
        void SendDroneToCharge(int droneId, int stationId);
        double ReleaseDroneFromCharge(int droneId, DateTime outTime, int stationId);

        Station GetStation(int id);
        Drone GetDrone(int id);
        Customer GetCustomer(int id);
        Package GetPackage(int id);

        void DeleteStation(int id);
        void DeletePackage(int id);
        void DeleteCustomer(int id);
        void DeleteDrone(int id);

        void UpdateStation(Station s);
        void UpdatePackage(Package p);
        void UpdateCustomer(Customer c);
        void UpdateDrone(Drone d);

        IEnumerable<Station> GetAllStations();
        IEnumerable<Drone> GetAllDrones();
        IEnumerable<Customer> GetAllCustomers();
        IEnumerable<Package> GetAllPackages();
        IEnumerable<Package> GetAllPackagesWhere(Func<Package, bool> func);
        IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate);
        IEnumerable<Drone> GetAllDronesWhere(Func<Drone, bool> predicate);
        IEnumerable<Customer> GetAllCustomerssWhere(Func<Customer, bool> predicate);

        double[] GetElectricity();

        bool isInCharge(int droneId);
    }
}
