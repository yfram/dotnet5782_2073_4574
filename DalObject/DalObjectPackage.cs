// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public partial class DalObject
    {
        public void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId,
            DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered)
        {
            DataSource.Packages.Add(new(DataSource.Config.RunNumber++, senderId, recevirId, weight, packagePriority, droneId,
Created, Associated, PickUp, Delivered));
        }

        public void DeliverPackage(int packageId)
        {

            int packageIndex = GetPackageIndex(packageId);
            int droneIndex = GetDroneIndex(DataSource.Packages[packageIndex].DroneId.Value);
            Drone tmp = DataSource.Drones[droneIndex];
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Packages.RemoveAt(packageIndex);
        }

        public Package GetPackage(int id)
        {
            int ix = GetPackageIndex(id);
            return ix == -1 ? throw new ArgumentException($"the Package {id} does not exist!") : DataSource.Packages[ix];
        }
        private static int GetPackageIndex(int id)
        {
            return DataSource.Packages.FindIndex(p => p.Id == id);
        }

        public IEnumerable<Package> GetAllPackages()
        {
            return new List<Package>(DataSource.Packages);
        }

        public IEnumerable<Package> GetAllPackagesWhere(Func<Package, bool> predicate)
        {
            return DataSource.Packages.Where(predicate);
        }
    }
}
