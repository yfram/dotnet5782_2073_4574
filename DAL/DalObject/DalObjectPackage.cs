﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IDAL.DO;


namespace DalObject
{
    public partial class DalObject
    {
        public void AddPackage(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int droneId,
            double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive) =>
            DataSource.Packages.Add(new(id, senderId, recevirId, weight, packagePriority, droneId,
                timeToPackage, timeToGetDrone, timeToGetPackedge, timeToRecive));

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

        public string GetPackageString(int id) => DataSource.Packages[GetPackageIndex(id)].ToString();

        public IEnumerable<Package> GetAllPackages() => new List<Package>(DataSource.Packages);
        public IEnumerable<Package> GetAllUndronedPackages() => DataSource.Packages.Where(p => p.DroneId == null);

        private int GetPackageIndex(int id) => DataSource.Packages.FindIndex(p => p.Id == id);

    }
}
