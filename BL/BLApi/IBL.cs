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
        void UpdateDroneName(int id, string newModel, double battery = -1 , Location newLocation = null);

        void UpdateStation(int id, string newName = "", int newChargeSlots = -1);

        void UpdateCustomer(int id, string newName = "", string newPhone = "");
        void SendDroneToCharge(int DroneId);

        void ReleaseDrone(int DroneId, System.DateTime time);

        void AssignPackage(int DroneId);

        void PickUpPackage(int DroneId, bool mode = false);

        void DeliverPackage(int DroneId , bool mode = false);

        #endregion

        #region Display Options

        Station DisplayStation(int StationId);
        Drone DisplayDrone(int DroneId);
        Customer DisplayCustomer(int CustomerId);

        Package DisplayPackage(int PackageId);

        #endregion

        #region Lists Display

        IEnumerable<StationForList> DisplayStations();
        IEnumerable<DroneForList> DisplayDrones();

        IEnumerable<CustomerForList> DisplayCustomers();
        IEnumerable<PackageForList> DisplayPackages();

        IEnumerable<PackageForList> DisplayPackagesWithoutDrone();

        IEnumerable<StationForList> DisplayStationsWithCharges();

        IEnumerable<dynamic> DisplayObjectsWhere<T>(Func<T, bool> func);

        #endregion
    }
}
