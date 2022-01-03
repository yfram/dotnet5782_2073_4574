using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dal
{
    public partial class DalXml : IDAL
    {
        private static readonly DalXml _Instance = new();
        public static DalXml Instance { get => _Instance; }

        public IEnumerable<T> ReadAllObjects<T>() where T : new()
        {
            var ObjectsRoot = XElement.Load($"Data/{typeof(T).Name}s.xml");

            List<T> ans = new List<T>();
            foreach (XElement elem in ObjectsRoot.Elements())
            {
                ans.Add(ReadObject<T>(elem));
            }
            return ans;

        }

        public IEnumerable<T> ReadAllObjectsWhen<T>(Func<T, bool> func) where T : new()
        {
            List<T> ans = new();
            IEnumerable<T> all = ReadAllObjects<T>();
            foreach (var elem in all)
            {
                if (func(elem))
                    ans.Add(elem);
            }
            return ans;
        }

        public void WriteAllObjects<T>(IEnumerable<T> write)
        {
            XElement root = new XElement("listOfObjects");

            foreach (T elem in write)
            {
                XElement current = new XElement(typeof(T).Name);
                foreach (PropertyInfo FI in typeof(T).GetProperties())
                {
                    current.Add(new XElement(FI.Name, FI.GetValue(elem)));
                }

                root.Add(current);
            }
            root.Save($"Data/{typeof(T).Name}s.xml");
        }

        public T ReadObject<T>(XElement elem) where T : new()
        {
            object current = new T();
            foreach (PropertyInfo FI in current.GetType().GetProperties())
            {
                if (Nullable.GetUnderlyingType(FI.PropertyType) is not null)
                {
                    if (elem.Element(FI.Name).Value.Trim().Length == 0)
                        FI.SetValue(current, null);
                    else
                        FI.SetValue(current, Convert.ChangeType(elem.Element(FI.Name).Value, Nullable.GetUnderlyingType(FI.PropertyType)));

                }
                else if (FI.PropertyType.IsEnum)
                {
                    FI.SetValue(current, Enum.Parse(FI.PropertyType, elem.Element(FI.Name).Value));
                }
                else
                    FI.SetValue(current, Convert.ChangeType(elem.Element(FI.Name).Value, FI.PropertyType));

            }
            return (T)current;
        }

        public T GetObject<T>(int id, string propName = "Id") where T : new()
        {
            var ObjectsRoot = XElement.Load($"Data/{typeof(T).Name}s.xml");

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
                throw new ArgumentException($"the property {propName} is not exsist in {typeof(T).Name} or it type is not int so it cant use as an id!");


            foreach (XElement elem in ObjectsRoot.Elements())
            {
                object obj = ReadObject<T>(elem);

                if ((int)prop.GetValue(obj) == id)
                    return (T)obj;
            }

            throw new ArgumentException($"the id {id} is not exsist!");
        }

        public void AddObject<T>(int id, T obj) where T : new()
        {
            try
            {
                GetObject<T>(id);
                throw new ArgumentException($"cannot add the {typeof(T)} with id {id} because it is exist");
            }
            catch (ArgumentException e)
            {
                WriteAllObjects<T>(ReadAllObjects<T>().Append(obj));
            }


        }

        public void DeleteObject<T>(int id, string propName = "Id") where T : new()
        {
            var all = ReadAllObjects<T>();
            IEnumerable<T> ans = new List<T>();

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
                throw new ArgumentException($"the property {propName} does not exsist in {typeof(T).Name} or its type is not int, so it can't be used as an id!");

            bool found = false;

            foreach (var elem in ans)
            {
                if ((int)prop.GetValue(elem) != id)
                    ans.Append(elem);
                else
                    found = true;
            }

            if (!found)
                throw new ArgumentException($"cannot find {id} of {typeof(T).Name}");

            WriteAllObjects(ans);
        }

        public void UpdateObject<T>(int id, T obj, string propName = "Id") where T : new()
        {
            var all = ReadAllObjects<T>();
            IEnumerable<T> ans = new List<T>();

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
                throw new ArgumentException($"the property {propName} is not exsist in {obj.GetType().Name} or it type is not int so it cant use as an id!");

            bool found = false;

            foreach (var elem in ans)
            {
                if ((int)prop.GetValue(elem) != id)
                    ans.Append(elem);
                else
                {
                    ans.Append(obj);
                    found = true;
                }
            }

            if (!found)
                throw new ArgumentException($"cannot find {id} of {typeof(T).Name}");

            WriteAllObjects(ans);
        }

        public void AddDrone(int id, string model, WeightGroup weight)
        {
            AddObject(id, new Drone(id, model, weight));
        }

        public void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId, DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered)
        {
            int runNumber = GetRunNumber();
            UpdateRunNumber();
            Package p = new Package(runNumber, senderId, recevirId, weight, packagePriority);
            p.Created = DateTime.Now;
            AddObject(runNumber, p);
        }

        private void UpdateRunNumber()
        {
            throw new NotImplementedException();
        }

        private int GetRunNumber()
        {
            throw new NotImplementedException();
        }

        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots)
        {
            AddObject(id, new Station(id, name, longitude, lattitude, chargeSlots));

        }



        public void DeleteDrone(int id)
        {
            DeleteObject<Drone>(id);
        }

        public void DeletePackage(int id)
        {
            DeleteObject<Package>(id);
        }

        public void DeleteStation(int id)
        {
            DeleteObject<Station>(id);
        }




        public IEnumerable<Drone> GetAllDrones()
        {
            return ReadAllObjects<Drone>();
        }

        public IEnumerable<Package> GetAllPackages()
        {
            return ReadAllObjects<Package>();
        }

        public IEnumerable<Package> GetAllPackagesWhere(Func<Package, bool> func)
        {
            return ReadAllObjectsWhen(func);
        }

        public IEnumerable<Station> GetAllStations()
        {
            return ReadAllObjects<Station>();
        }

        public IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate)
        {
            return ReadAllObjectsWhen(predicate);
        }



        public Drone GetDrone(int id)
        {
            return GetObject<Drone>(id);
        }

        public double[] GetElectricity()
        {
            double[] ans = new double[] { 0, 0, 0, 0 };//DataSource.Config.ElecEmpty, DataSource.Config.ElecLow, DataSource.Config.ElecMid, DataSource.Config.ElecHigh, DataSource.Config.ElecRatePercent };
            return ans;
        }

        public Package GetPackage(int id)
        {
            return GetObject<Package>(id);
        }

        public Station GetStation(int id)
        {
            return GetObject<Station>(id);
        }

        public void GivePackageDrone(int packageId, int droneId)
        {
            var p = GetObject<Package>(packageId);
            var d = GetObject<Drone>(droneId); // only check if exist

            p.DroneId = droneId;
            p.Associated = DateTime.Now;
        }

        public void PickUpPackage(int packageId)
        {
            var p = GetObject<Package>(packageId);

            p.PickUp = DateTime.Now;
        }

        public void DeliverPackage(int packageId)
        {
            var p = GetObject<Package>(packageId);
            p.Delivered = DateTime.Now;
        }

        public double ReleaseDroneFromCharge(int droneId, DateTime outDate, int stationId)
        {
            Station s = GetObject<Station>(stationId);
            s.ChargeSlots += 1;
            UpdateObject(stationId, s);
            double ans = outDate.Subtract(GetObject<DroneCharge>(droneId, "DroneId").Enter).TotalSeconds;
            DeleteObject<DroneCharge>(droneId, "DroneId");
            return ans;
        }

        public void SendDroneToCharge(int droneId, int stationId)
        {
            Station s = GetObject<Station>(stationId);
            if (s.ChargeSlots <= 0)
                throw new ArgumentException($"cannot send the drone {droneId} to charge at {stationId} because it hass only {s.ChargeSlots} empty slots!");
            s.ChargeSlots -= 1;
            UpdateObject(stationId, s);
            DroneCharge d = new DroneCharge(droneId, stationId , DateTime.Now);
            AddObject<DroneCharge>(droneId , d);
        }



        public void UpdateDrone(Drone d)
        {
            UpdateObject(d.Id, d);
        }

        public void UpdatePackage(Package p)
        {
            UpdateObject(p.Id, p);
        }

        public void UpdateStation(Station s)
        {
            UpdateObject(s.Id, s);
        }
    }
}
