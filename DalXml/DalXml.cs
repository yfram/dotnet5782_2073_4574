// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Dal
{
    public partial class DalXml : IDAL
    {
        private static readonly DalXml _Instance = new();
        public static DalXml InstanceXml { get => _Instance; }

        public struct Config
        {
            public int runNumber;
            public double ElecEmpty, ElecLow, ElecMid, ElecHigh, ElecRatePercent;
        }

        public bool isInCharge(int droneId)
        {
            return ReadAllObjects<DroneCharge>().Where(s => s.DroneId == droneId).Any();
        }

        public static IEnumerable<T> ReadAllObjects<T>() where T : new()
        {
            var ObjectsRoot = XElement.Load($"Data/{typeof(T).Name}s.xml");

            List<T> ans = new();
            foreach (XElement elem in ObjectsRoot.Elements())
            {
                ans.Add(ReadObject<T>(elem));
            }
            return ans;

        }

        public static IEnumerable<T> ReadAllObjectsWhen<T>(Func<T, bool> func) where T : new()
        {
            List<T> ans = new();
            IEnumerable<T> all = ReadAllObjects<T>();
            foreach (var elem in all)
            {
                if (func(elem))
                {
                    ans.Add(elem);
                }
            }
            return ans;
        }

        public static void WriteAllObjects<T>(IEnumerable<T> write)
        {
            XElement root = new("listOfObjects");

            foreach (T elem in write)
            {
                XElement current = new(typeof(T).Name);
                foreach (PropertyInfo FI in typeof(T).GetProperties())
                {
                    current.Add(new XElement(FI.Name, FI.GetValue(elem)));
                }

                root.Add(current);
            }
            root.Save($"Data/{typeof(T).Name}s.xml");
        }

        public static T ReadObject<T>(XElement elem) where T : new()
        {
            object current = new T();
            foreach (PropertyInfo FI in current.GetType().GetProperties())
            {
                if (Nullable.GetUnderlyingType(FI.PropertyType) is not null)
                {
                    if (elem.Element(FI.Name).Value.Trim().Length == 0)
                    {
                        FI.SetValue(current, null);
                    }
                    else
                    {
                        FI.SetValue(current, Convert.ChangeType(elem.Element(FI.Name).Value, Nullable.GetUnderlyingType(FI.PropertyType)));
                    }
                }
                else if (FI.PropertyType.IsEnum)
                {
                    FI.SetValue(current, Enum.Parse(FI.PropertyType, elem.Element(FI.Name).Value));
                }
                else
                {
                    FI.SetValue(current, Convert.ChangeType(elem.Element(FI.Name).Value, FI.PropertyType));
                }
            }
            return (T)current;
        }

        public static T GetObject<T>(int id, string propName = "Id") where T : new()
        {
            var ObjectsRoot = XElement.Load($"Data/{typeof(T).Name}s.xml");

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
            {
                throw new ArgumentException($"the property {propName} is not exist in {typeof(T).Name} or it type is not int so it cant use as an id!");
            }

            foreach (XElement elem in ObjectsRoot.Elements())
            {
                object obj = ReadObject<T>(elem);

                if ((int)prop.GetValue(obj) == id)
                {
                    return (T)obj;
                }
            }

            throw new ArgumentException($"the id {id} is not exist!");
        }

        public static void AddObject<T>(int id, T obj) where T : new()
        {
            try
            {
                GetObject<T>(id);
                throw new ArgumentException($"cannot add the {typeof(T)} with id {id} because it is exist");
            }
            catch (ArgumentException)
            {
                WriteAllObjects<T>(ReadAllObjects<T>().Append(obj));
            }
            catch (FileNotFoundException)
            {
                var param = new List<T>() { obj };
                WriteAllObjects<T>(param);

            }

        }

        public static void DeleteObject<T>(int id, string propName = "Id") where T : new()
        {
            var all = ReadAllObjects<T>();
            IEnumerable<T> ans = new List<T>();

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
            {
                throw new ArgumentException($"the property {propName} does not exist in {typeof(T).Name} or its type is not int, so it can't be used as an id!");
            }

            bool found = false;

            foreach (var elem in all)
            {
                if ((int)prop.GetValue(elem) != id)
                {
                    ans = ans.Append(elem);
                }
                else
                {
                    found = true;
                }
            }

            if (!found)
            {
                throw new ArgumentException($"cannot find {id} of {typeof(T).Name}");
            }

            WriteAllObjects(ans);
        }

        public static void UpdateObject<T>(int id, T obj, string propName = "Id") where T : new()
        {
            var all = ReadAllObjects<T>();
            IEnumerable<T> ans = new List<T>();

            var prop = typeof(T).GetProperty(propName);
            if (prop is null || prop.PropertyType != typeof(int))
            {
                throw new ArgumentException($"the property {propName} is not exist in {obj.GetType().Name} or it type is not int so it cant use as an id!");
            }

            bool found = false;

            foreach (var elem in all)
            {
                if ((int)prop.GetValue(elem) != id)
                {
                    ans = ans.Append(elem);
                }
                else
                {
                    ans = ans.Append(obj);
                    found = true;
                }
            }

            if (!found)
            {
                throw new ArgumentException($"cannot find {id} of {typeof(T).Name}");
            }

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
            Package p = new(runNumber, senderId, recevirId, weight, packagePriority);
            p.Created = DateTime.Now;
            p.DroneId = null;
            AddObject(runNumber, p);
        }

        private static void UpdateRunNumber()
        {
            var cfg = ReadConfigFile();
            cfg.runNumber += 1;
            WriteConfigFile(cfg);
        }

        private static int GetRunNumber()
        {
            return ReadConfigFile().runNumber;
        }

        private static Config ReadConfigFile()
        {
            var serializer = new XmlSerializer(typeof(Config));
            using var reader = XmlReader.Create("Data/config.xml");
            return (Config)serializer.Deserialize(reader);
        }

        private static void WriteConfigFile(Config data)
        {
            var serializer = new XmlSerializer(typeof(Config));
            using var writer = XmlWriter.Create("Data/config.xml");
            serializer.Serialize(writer, data);
        }
        public void AddStation(int id, string name, double longitude, double latitude, int chargeSlots)
        {
            AddObject(id, new Station(id, name, longitude, latitude, chargeSlots));

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

        public IEnumerable<Drone> GetAllDronesWhere(Func<Drone, bool> predicate)
        {
            return ReadAllObjectsWhen(predicate);
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
            Config c = ReadConfigFile();
            double[] ans = new double[] { c.ElecEmpty, c.ElecLow, c.ElecMid, c.ElecHigh, c.ElecRatePercent };
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
            _ = GetObject<Drone>(droneId); //TODO: add catch

            p.DroneId = droneId;
            p.Associated = DateTime.Now;

            UpdateObject<Package>(p.Id, p);
        }

        public void PickUpPackage(int packageId)
        {
            var p = GetObject<Package>(packageId);

            p.PickUp = DateTime.Now;
            UpdateObject<Package>(p.Id, p);
        }

        public void DeliverPackage(int packageId)
        {
            var p = GetObject<Package>(packageId);
            p.Delivered = DateTime.Now;
            UpdateObject<Package>(p.Id, p);
        }

        public double ReleaseDroneFromCharge(int droneId, DateTime outDate, int stationId)
        {

            Station s = stationId > -1
                ? GetObject<Station>(stationId)
                : GetObject<Station>(ReadAllObjects<DroneCharge>().Where(d => d.DroneId == droneId).ElementAt(0).StationId);
            s.ChargeSlots += 1;
            stationId = s.Id;
            UpdateObject(stationId, s);
            double ans = outDate.Subtract(GetObject<DroneCharge>(droneId, "DroneId").Enter).TotalSeconds;
            DeleteObject<DroneCharge>(droneId, "DroneId");
            return ans;
        }

        public void SendDroneToCharge(int droneId, int stationId)
        {
            Station s = GetObject<Station>(stationId);
            if (s.ChargeSlots <= 0)
            {
                throw new ArgumentException($"cannot send the drone {droneId} to charge at {stationId} because it sash only {s.ChargeSlots} empty slots!");
            }

            s.ChargeSlots -= 1;
            UpdateObject(stationId, s);
            DroneCharge d = new(droneId, stationId, DateTime.Now);
            AddObject(droneId, d);
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

        public IEnumerable<Customer> GetAllCustomerssWhere(Func<Customer, bool> predicate)
        {
            return ReadAllObjectsWhen<Customer>(predicate);
        }
    }
}
