using BO;
using System;
using System.Collections.Generic;

namespace BlApi
{
    public interface IBL
    {

        void StartSimulator(int DroneId, Action update, Func<bool> stop);

        #region Add Options
        void AddStation(Station s);
        void AddDrone(Drone s);
        void AddCustomer(Customer c);
        void AddPackage(Package p);
        void AddPackage(int sid, int rid, BO.WeightGroup weight, BO.PriorityGroup packagePriority);
        #endregion

        #region Update Options
        void UpdateDroneName(int id, string newModel, double battery = -1, Location newLocation = null);
        void SendDroneToCharge(int DroneId);
        void ReleaseDrone(int DroneId, System.DateTime time);
        void AssignPackage(int DroneId);
        void PickUpPackage(int DroneId, bool mode = false);
        void DeliverPackage(int DroneId, bool mode = false);

        void UpdateStation(int id, string newName = "", int newChargeSlots = -1);
        void UpdateCustomer(int id, string newName = "", string newPhone = "");
        #endregion

        #region Display Options
        Station GetStationById(int StationId);
        Drone GetDroneById(int DroneId);
        Customer GetCustomerById(int CustomerId);
        Package GetPackageById(int PackageId);
        #endregion

        #region Lists Display

        IEnumerable<StationForList> GetAllStations();
        IEnumerable<DroneForList> GetAllDrones();
        IEnumerable<CustomerForList> GetAllCustomers();
        IEnumerable<PackageForList> GetAllPackages();
        IEnumerable<StationForList> GetAllStationsWhere(Func<StationForList, bool> func);
        IEnumerable<DroneForList> GetAllDronesWhere(Func<DroneForList, bool> func);
        IEnumerable<CustomerForList> GetAllCustomersWhere(Func<CustomerForList, bool> func);
        IEnumerable<PackageForList> GetAllPackagesWhere(Func<PackageForList, bool> func);

        IEnumerable<PackageForList> GetPackagesWithoutDrone();
        IEnumerable<StationForList> GetStationsWithCharges();

        IEnumerable<dynamic> GetObjectsWhere<T>(Func<T, bool> func);

        #endregion

        void DeletePackage(int packageId);

    }
}
