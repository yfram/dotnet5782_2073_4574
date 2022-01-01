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
            try
            {
                var ObjectsRoot = XElement.Load($"Data/{typeof(T).Name}s.xml");

                List<T> ans = new List<T>();
                foreach(var elem in ObjectsRoot.Elements())
                {
                    object current = new T();
                    foreach (PropertyInfo FI in current.GetType().GetProperties())
                    {
                        if (FI.PropertyType == typeof(int))
                        {
                            FI.SetValue(current, int.Parse(elem.Element(FI.Name).Value), null);
                        }
                        else if (FI.PropertyType == typeof(double))
                        {
                            FI.SetValue(current, Double.Parse(elem.Element(FI.Name).Value), null);
                        }
                        else if (FI.PropertyType == typeof(String))
                        {
                            FI.SetValue(current, elem.Element(FI.Name).Value, null);
                        }
                        else if (FI.PropertyType.IsEnum)
                        {
                            FI.SetValue(current, int.Parse(elem.Element(FI.Name).Value, null));
                        }
                        else
                            throw new ArgumentException($"the type {current.GetType().Name} has field that aren't implement yet:{FI.PropertyType.Name}  {FI.Name}");


                    }
                    ans.Add((T)current);
                }
                return ans;
                
            }
            catch { return null; }
        }

        public void WriteAllObjects<T>(IEnumerable<T> write)
        {
            XElement root = new XElement("listOfObjects");

            foreach(T elem in write)
            {
                XElement current = new XElement(typeof(T).Name);
                foreach(PropertyInfo FI in typeof(T).GetProperties())
                {
                    current.Add(new XElement(FI.Name, FI.GetValue(elem)));
                }

                root.Add(current);
            }
            root.Save($"Data/{typeof(T).Name}s.xml");
        }

        public void AddDrone(int id, string model, WeightGroup weight)
        {

        }

        public void AddPackage(int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId, DateTime? Created, DateTime? Associated, DateTime? PickUp, DateTime? Delivered)
        {
            throw new NotImplementedException();
        }

        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots)
        {
            throw new NotImplementedException();
        }



        public void DeleteDrone(int id)
        {
            throw new NotImplementedException();
        }

        public void DeletePackage(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteStation(int id)
        {
            throw new NotImplementedException();
        }

        public void DeliverPackage(int packageId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return ReadAllObjects<Customer>();
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
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetAllStations()
        {
            return ReadAllObjects<Station>();
        }

        public IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate)
        {
            throw new NotImplementedException();
        }



        public Drone GetDrone(int id)
        {
            throw new NotImplementedException();
        }

        public double[] GetElectricity()
        {
            double[] ans = new double[] { 0, 0, 0, 0 };//DataSource.Config.ElecEmpty, DataSource.Config.ElecLow, DataSource.Config.ElecMid, DataSource.Config.ElecHigh, DataSource.Config.ElecRatePercent };
            return ans;
        }

        public Package GetPackage(int id)
        {
            throw new NotImplementedException();
        }

        public Station GetStation(int id)
        {
            throw new NotImplementedException();
        }

        public void GivePackageDrone(int packageId, int droneId)
        {
            throw new NotImplementedException();
        }

        public void PickUpPackage(int packageId, int droneID)
        {
            throw new NotImplementedException();
        }

        public void ReleaseDroneFromCharge(int droneId, int stationId)
        {
            throw new NotImplementedException();
        }

        public void SendDroneToCharge(int droneId, int stationId)
        {
            throw new NotImplementedException();
        }



        public void UpdateDrone(Drone d)
        {
            throw new NotImplementedException();
        }

        public void UpdatePackage(Package p)
        {
            throw new NotImplementedException();
        }

        public void UpdateStation(Station s)
        {
            throw new NotImplementedException();
        }
    }
}
