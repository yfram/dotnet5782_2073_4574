﻿using System;
using System.Collections.Generic;
using System.Linq;
using IDAL.DO;

namespace DalObject
{
    public class DalObject
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        #region Add functions
        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots) =>
            DataSource.Stations.Add(new(id, name, longitude, lattitude, chargeSlots));
        public void AddDrone(int id, string model, double charge, WeightGroup weight, DroneStates state) =>
            DataSource.Drones.Add(new(id, model, charge, weight, state));
        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude) =>
            DataSource.Customers.Add(new(id, name, phone, lattitude, longitude));
        public void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive) =>
            DataSource.Packages.Add(new(id, senderId, recevirId, weight, packagePriority, droneId,
                timeToPackage, timeToGetDrone, timeToGetPackedge, timeToRecive));
        #endregion

        #region Update functions
        public void GivePackageDrone(Package package, Drone drone) => package.DroneId = drone.Id;
        public void PickUpPackage(Package package, Drone drone)
        {
            GivePackageDrone(package, drone);
            drone.State = DroneStates.Shipping;
        }
        public void DeliverPackadge(Drone drone, Package package)
        {
            drone.State = DroneStates.Empty;
            DataSource.Packages.Remove(package);
            //DataSource.Customers.RemoveAll(c => c.Id == package.RecevirId); was not sure this is needed, we might want to save a list of past customers
        }
        public void SendDroneToCharge(Drone drone, Station station)
        {
            drone.State = DroneStates.Maintenance;//I think its Maintenance, should be at least
            station.ChargeSlots--;
        }
        public void ReleeseDroneFromCharge(Drone drone, Station station)
        {
            drone.State = DroneStates.Empty;
            station.ChargeSlots++;
        }
        #endregion
    }
}