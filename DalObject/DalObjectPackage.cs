// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dal
{
    public partial class DalObject
    {
        /// <summary>
        /// Add a new package to the data base
        /// </summary>
        /// <param name="senderId">Sender's ID for the new package</param>
        /// <param name="recevirId">Receiver's ID for the new package</param>
        /// <param name="weight">Weight group for the new package</param>
        /// <param name="packagePriority">Priority group for the new package</param>
        /// <param name="droneId">The id for the paired drone of the new package</param>
        /// <param name="Created">Time the package was created</param>
        /// <param name="Associated">Time the package was associated</param>
        /// <param name="PickUp">Time the package was picked up</param>
        /// <param name="Delivered">Time the package was delivered</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId,
            DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered)
        {
            DataSource.Packages.Add(new(DataSource.Config.RunNumber++, senderId, recevirId, weight, packagePriority, droneId,
Created, Associated, PickUp, Delivered));
        }

        /// <summary>
        /// Gets the package with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted package</param>
        /// <returns>The package with id <paramref name="id"/></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeliverPackage(int packageId)
        {

            int packageIndex = GetPackageIndex(packageId);
            int droneIndex = GetDroneIndex(DataSource.Packages[packageIndex].DroneId.Value);
            Drone tmp = DataSource.Drones[droneIndex];
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Packages.RemoveAt(packageIndex);
        }

        /// <summary>
        /// Get the index of the package with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted package</param>
        /// <returns>The index of the package with id <paramref name="id"/></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Package GetPackage(int id)
        {
            int ix = GetPackageIndex(id);
            return ix == -1 ? throw new ArgumentException($"the Package {id} does not exist!") : DataSource.Packages[ix];
        }

        /// <summary>
        /// Get the index of the package with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted package</param>
        /// <returns>The index of the package with id <paramref name="id"/></returns>
        private int GetPackageIndex(int id)
        {
            return DataSource.Packages.FindIndex(p => p.Id == id);
        }

        /// <summary>
        /// Gets all packages in the data base
        /// </summary>
        /// <returns>A IEnumerable with all the packages in the data base</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> GetAllPackages()
        {
            return new List<Package>(DataSource.Packages);
        }

        /// <summary>
        /// Gets all packages in the data base that answer to predicate
        /// </summary>
        /// <param name="predicate">The function to filter packages with</param>
        /// <returns>A IEnumerable with all the packages in the data base that answer to predicate</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Package> GetAllPackagesWhere(Func<Package, bool> predicate)
        {
            return DataSource.Packages.Where(predicate);
        }
    }
}
