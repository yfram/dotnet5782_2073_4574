// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Runtime.CompilerServices;

namespace Dal
{
    public partial class DalObject : DalApi.IDAL
    {
        private static readonly DalObject _Instance = new();
        public static DalObject InstanceObject { get => _Instance; }

        static DalObject()
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// Returns the charging status of the drone <paramref name="droneId"/>
        /// </summary>
        /// <param name="droneId">ID of the wanted drone</param>
        /// <returns><code>true</code>If the drone is charging<code>false</code>If the drone is not charging</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool isInCharge(int droneId)
        {
            return DataSource.DroneCharges.Exists(dc => dc.DroneId == droneId);
        }

        /// <summary>
        /// Get DalObject's charging data
        /// </summary>
        /// <returns>An array that contains all of DalObject's charging data</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double[] GetElectricity()
            => new double[] { DataSource.Config.ElecEmpty, DataSource.Config.ElecLow, DataSource.Config.ElecMid, DataSource.Config.ElecHigh, DataSource.Config.ElecRatePercent
    };

        #region Update functions

        /// <summary>
        /// Updates the station with the same id as <paramref name="s"/> to be <paramref name="s"/>
        /// </summary>
        /// <param name="s">The new station</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateStation(Station s)
        {
            int ix = GetStationIndex(s.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Station {s.Id} does not exist!");
            }

            DataSource.Stations[ix] = s;
        }

        /// <summary>
        /// Updates the package with the same id as <paramref name="p"/> to be <paramref name="p"/>
        /// </summary>
        /// <param name="s">The new package</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdatePackage(Package p)
        {
            int ix = GetPackageIndex(p.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Package {p.Id} does not exist!");
            }

            DataSource.Packages[ix] = p;
        }

        /// <summary>
        /// Updates the customer with the same id as <paramref name="c"/> to be <paramref name="c"/>
        /// </summary>
        /// <param name="c">The new customer</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateCustomer(Customer c)
        {
            int ix = GetCustomerIndex(c.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Customer {c.Id} does not exist!");
            }

            DataSource.Customers[ix] = c;
        }

        /// <summary>
        /// Updates the drone with the same id as <paramref name="d"/> to be <paramref name="d"/>
        /// </summary>
        /// <param name="d">The new drone</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void UpdateDrone(Drone d)
        {
            int ix = GetDroneIndex(d.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Drone {d.Id} does not exist!");
            }

            DataSource.Drones[ix] = d;
        }
        #endregion

        #region Complex update functions

        /// <summary>
        /// Associate the package with id <paramref name="packageId"/> to the drone with id <paramref name="droneId"/>
        /// </summary>
        /// <param name="packageId">The package to associate</param>
        /// <param name="droneId">The drone to associate to</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void GivePackageDrone(int packageId, int droneId)
        {
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.DroneId = droneId;
            tmp.Associated = DateTime.Now;
            DataSource.Packages[index] = tmp;

        }

        /// <summary>
        /// Picks up the package with id <paramref name="packageId"/>
        /// </summary>
        /// <param name="packageId">The id for the package to pick up</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void PickUpPackage(int packageId)
        {
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.PickUp = DateTime.Now;
            DataSource.Packages[index] = tmp;
        }

        /// <summary>
        /// Sends the drone with id <paramref name="droneId"/> to charge at the station with id <paramref name="stationId"/>
        /// </summary>
        /// <param name="droneId">The id for the drone to charge</param>
        /// <param name="stationId">The id for the station to send to</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendDroneToCharge(int droneId, int stationId)
        {
            int droneIndex = GetDroneIndex(droneId);
            Drone tmp = DataSource.Drones[droneIndex];
            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];
            tmp1.ChargeSlots--;
            DataSource.DroneCharges.Add(new(droneId, stationId, DateTime.Now));
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Stations[stationIndex] = tmp1;
        }

        /// <summary>
        /// Releases the drone with id <paramref name="droneId"/> from charging at <paramref name="outTime"/> 
        /// </summary>
        /// <param name="droneId">The id for the drone to be released</param>
        /// <param name="outTime">The time to release at</param>
        /// <param name="stationId">The id of the station that <paramref name="droneId"/> is at</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public double ReleaseDroneFromCharge(int droneId, DateTime outTime, int stationId = -1)
        {
            double ans = 0;
            if (stationId < 0)
            {
                stationId = DataSource.DroneCharges.Find(dc => dc.DroneId == droneId).StationId;
            }

            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];
            tmp1.ChargeSlots++;

            DataSource.Stations[stationIndex] = tmp1;
            if (!DataSource.DroneCharges.Exists(d => d.DroneId == droneId && d.StationId == stationId))
            {
                throw new ArgumentException($"cannot find the droncharge with the drone id {droneId}");
            }

            ans = outTime.Subtract(DataSource.DroneCharges.Find(d => d.DroneId == droneId && d.StationId == stationId).Enter).TotalSeconds;
            DataSource.DroneCharges.RemoveAll(d => d.DroneId == droneId && d.StationId == stationId);
            return ans;
        }
        #endregion

        #region Delete functions

        /// <summary>
        /// Delete the station with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the station to delete</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteStation(int id)
        {
            int ix = GetStationIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Station {id} does not exist!");
            }

            DataSource.Stations.RemoveAt(ix);

        }

        /// <summary>
        /// Delete the package with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the package to delete</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeletePackage(int id)
        {
            int ix = GetPackageIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Package {id} does not exist!");
            }

            DataSource.Packages.RemoveAt(ix);
        }

        /// <summary>
        /// Delete the customer with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the station to delete</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Customer {id} does not exist!");
            }

            DataSource.Customers.RemoveAt(ix);
        }

        /// <summary>
        /// Delete the drone with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">Id of the drone to delete</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void DeleteDrone(int id)
        {
            int ix = GetDroneIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Drone {id} does not exist!");
            }

            DataSource.Drones.RemoveAt(ix);
        }
        #endregion
    }
}
