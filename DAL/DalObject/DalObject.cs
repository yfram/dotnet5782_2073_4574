using DO;
using System;

namespace DalObject
{
    public partial class DalObject : DalApi.IDAL
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        #region Update functions
        public void GivePackageDrone(int packageId, int droneId)
        {
            //this function could be one line, but Package is a struct, so that would not work. I hate structs.
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.DroneId = droneId;
            tmp.Associated = DateTime.Now;
            DataSource.Packages[index] = tmp;

        }
        public void PickUpPackage(int packageId, int droneID)
        {
            int index = GetPackageIndex(packageId);
            Package tmp = DataSource.Packages[index];
            tmp.PickUp = DateTime.Now;
            DataSource.Packages[index] = tmp;
            /*
            int index = GetDroneIndex(droneID);
            Drone tmp = DataSource.Drones[index];
            //tmp.State = DroneStates.Shipping;
            DataSource.Drones[index] = tmp;
            */
        }

        /*
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
        */
        public void SendDroneToCharge(int droneId, int stationId)
        {
            int droneIndex = GetDroneIndex(droneId);
            Drone tmp = DataSource.Drones[droneIndex];
            //tmp.State = DroneStates.Maintenance;//I think its Maintenance, should be at least
            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];
            tmp1.ChargeSlots--;
            DataSource.DroneCharges.Add(new(droneId, stationId));
            DataSource.Drones[droneIndex] = tmp;
            DataSource.Stations[stationIndex] = tmp1;
        }
        public void ReleaseDroneFromCharge(int droneId, int stationId = -1)
        {
            //int droneIndex = GetDroneIndex(droneId);
            //Drone tmp = DataSource.Drones[droneIndex];

            //tmp.State = DroneStates.Empty;//I think its Maintenance, should be at least

            if (stationId < 0)
            {
                stationId = DataSource.DroneCharges.Find(dc => dc.DroneId == droneId).StationId;
            }

            int stationIndex = GetStationIndex(stationId);
            Station tmp1 = DataSource.Stations[stationIndex];

            tmp1.ChargeSlots++;

            //DataSource.Drones[droneIndex] = tmp;
            DataSource.Stations[stationIndex] = tmp1;

            DataSource.DroneCharges.RemoveAll(d => d.DroneId == droneId && d.StationId == stationId);
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
                throw new ArgumentException($"the Station {id} does not exist!");
            DataSource.Stations.RemoveAt(ix);

        }

        public void DeletePackage(int id)
        {
            int ix = GetPackageIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Package {id} does not exist!");
            DataSource.Packages.RemoveAt(ix);
        }

        public void DeleteCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Customer {id} does not exist!");
            DataSource.Customers.RemoveAt(ix);
        }

        public void DeleteDrone(int id)
        {
            int ix = GetDroneIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Drone {id} does not exist!");
            DataSource.Drones.RemoveAt(ix);
        }

        public void UpdateStation(Station s)
        {
            int ix = GetStationIndex(s.Id);
            if (ix == -1)
                throw new ArgumentException($"the Station {s.Id} does not exist!");
            DataSource.Stations[ix] = s;
        }

        public void UpdatePackage(Package p)
        {
            int ix = GetPackageIndex(p.Id);
            if (ix == -1)
                throw new ArgumentException($"the Package {p.Id} does not exist!");
            DataSource.Packages[ix] = p;
        }

        public void UpdateCustomer(Customer c)
        {
            int ix = GetCustomerIndex(c.Id);
            if (ix == -1)
                throw new ArgumentException($"the Customer {c.Id} does not exist!");
            DataSource.Customers[ix] = c;
        }

        public void UpdateDrone(Drone d)
        {
            int ix = GetDroneIndex(d.Id);
            if (ix == -1)
                throw new ArgumentException($"the Drone {d.Id} does not exist!");
            DataSource.Drones[ix] = d;
        }
    }
}
