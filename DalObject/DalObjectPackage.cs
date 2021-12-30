using DO;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Dal
{
    public partial class DalObject
    {
        public void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId,
            DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered) =>

            DataSource.Packages.Add(new(DataSource.Config.RunNumber++, senderId, recevirId, weight, packagePriority, droneId,
                Created, Associated, PickUp, Delivered));

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

        public Package GetPackage(int id)
        {
            int ix = GetPackageIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Package {id} does not exist!");
            return DataSource.Packages[ix];

        }

        public IEnumerable<Package> GetAllPackages() => new List<Package>(DataSource.Packages);
        public IEnumerable<Package> GetAllPackagesWhere(Func<Package, bool> predicate) => DataSource.Packages.Where(predicate);

        private int GetPackageIndex(int id) => DataSource.Packages.FindIndex(p => p.Id == id);

    }
}
