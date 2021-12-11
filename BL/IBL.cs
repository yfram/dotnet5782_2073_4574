using BO;
using System.Collections.Generic;

namespace BlApi
{
    public interface IBL
    {

        #region Add Options
        void AddStation(Station s);

        void AddDrone(Drone s);

        void AddCustomer(Customer c);

        void AddPackage(Package p);

        #endregion

        #region Update Options
        void UpdateDroneName(int id, string newModel);

        void UpdateStation(int id, string newName = "", int newChargeSlots = -1);

        void UpdateCustomer(int id, string newName = "", string newPhone = "");
        void SendDroneToCharge(int DroneId);

        void ReleaseDrone(int DroneId, double time);

        void AssignPackage(int DroneId);

        void PickUpPackage(int DroneId);

        void DeliverPackage(int DroneId);

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


        #endregion
    }
}
