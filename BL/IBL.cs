﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IBL.BO; 

namespace IBL
{
    public interface IBL
    {

        #region Add Options
        void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots);

        void AddDrone(int id, string model, WeightGroup weight, int stationId);

        void AddCustomer(int id, string name, string phone, Location loc);

        void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, PriorityGroup packagePriority);

        #endregion

        #region Update Options
        void UpdateDroneName(int id, string newModel);

        void UpdateStation(int id, string newName="", int newChargeSlots=-1);

        void UpdateCustomer(int id, string newName = "", int newPhone = -1);
        void SendDroneToCharge(int DroneId);

        void ReleaseDrone(int DroneId, double time);

        void AssignPackage(int DroneId);

        void PickPackage(int DroneId);

        void GivePackage(int DroneId);

        #endregion

        #region Display Options

        string DisplayStation(int StationId);
        string DisplayDrone(int DroneId);
        string DisplayCustomer(int CustomerId);

        string DisplayPackage(int PackageId);

        #endregion


        #region Lists Display

        string DisplayStations();
        string DisplayDrones();

        string DisplayCustomers();
        string DisplayPackages();

        string DisplayPackagesWithoutDrone();

        string DisplayStationsWithCharges();


        #endregion
    }
}
