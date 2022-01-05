using DO;
using System;

namespace Dal
{
    public partial class DalObject : DalApi.IDAL
    {
        private static readonly DalObject _Instance = new();
        public static DalObject Instance { get => _Instance; }

        static DalObject()
        {
            DataSource.Initialize();
        }

        public bool isInCharge(int droneId)
        {
            return DataSource.DroneCharges.Exists(dc => dc.DroneId == droneId);
        }

        #region Update functions
        public void GivePackageDrone(int packageId, int droneId)
        {
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.DroneId = droneId;
            tmp.Associated = DateTime.Now;
            DataSource.Packages[index] = tmp;

        }
        public void PickUpPackage(int packageId)
        {
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.PickUp = DateTime.Now;
            DataSource.Packages[index] = tmp;
        }

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

        public double[] GetElectricity()
        {
            double[] ans = new double[] { DataSource.Config.ElecEmpty, DataSource.Config.ElecLow, DataSource.Config.ElecMid, DataSource.Config.ElecHigh, DataSource.Config.ElecRatePercent };
            return ans;
        }

        public void DeleteStation(int id)
        {
            int ix = GetStationIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Station {id} does not exist!");
            }

            DataSource.Stations.RemoveAt(ix);

        }

        public void DeletePackage(int id)
        {
            int ix = GetPackageIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Package {id} does not exist!");
            }

            DataSource.Packages.RemoveAt(ix);
        }

        public void DeleteCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Customer {id} does not exist!");
            }

            DataSource.Customers.RemoveAt(ix);
        }

        public void DeleteDrone(int id)
        {
            int ix = GetDroneIndex(id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Drone {id} does not exist!");
            }

            DataSource.Drones.RemoveAt(ix);
        }

        public void UpdateStation(Station s)
        {
            int ix = GetStationIndex(s.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Station {s.Id} does not exist!");
            }

            DataSource.Stations[ix] = s;
        }

        public void UpdatePackage(Package p)
        {
            int ix = GetPackageIndex(p.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Package {p.Id} does not exist!");
            }

            DataSource.Packages[ix] = p;
        }

        public void UpdateCustomer(Customer c)
        {
            int ix = GetCustomerIndex(c.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Customer {c.Id} does not exist!");
            }

            DataSource.Customers[ix] = c;
        }

        public void UpdateDrone(Drone d)
        {
            int ix = GetDroneIndex(d.Id);
            if (ix == -1)
            {
                throw new ArgumentException($"the Drone {d.Id} does not exist!");
            }

            DataSource.Drones[ix] = d;
        }
    }
}
