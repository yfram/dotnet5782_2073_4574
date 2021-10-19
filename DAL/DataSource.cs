using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
    public class DataSource
    {

        internal class Config
        {
            //using Lists, so no need for the rest
            internal static int RunNumber = 0;
        }

        private static readonly List<string> DroneModels = new List<string>()
        {
            "DJIM1", "DJIF3", "Mavic Pro", "DJI Phantom 4","DJI Mavic 2 Zoom", "DJI Mavic 2 Pro"
        };
        private static readonly List<string> StationNames = new List<string>()
        {
            "Tel Aviv", "Bear Sheava", "Modiein", "Jeruselam","Heifa"
        };
        private static readonly List<string> CustomerNames = new List<string>()
        {
            "Yoni", "Gil", "Guy","Gal","Pete","Winston","Nick","Neo","Morpheus", "Trinity"
        };

        internal static List<Drone> Drones = new(10);
        internal static List<Station> Stations = new(5);
        internal static List<Customer> Customers = new(100);
        internal static List<Package> Packages = new(1000);
        internal static Config Configuration { get; set; }

        public static void Initialize()
        {
            Configuration = new Config();  //Config is all 0's anyways
            Random random = new Random();
            for (int i = 0; i < 2; i++)
                Stations.Add(InitStation(i, random));
            for (int i = 0; i < 5; i++)
                Drones.Add(InitDrone(i, random));
            for (int i = 0; i < 10; i++)
                Customers.Add(InitCustumer(i, random));
            for (int i = 0; i < 10; i++)
                Packages.Add(InitPackage(i, random));
        }

        private static Package InitPackage(int i, Random random) => new(i, Customers[i].Id, Customers[random.Next() % 10].Id, (WeightGroup)random.Next(), (Priority)(i % 3),
           null, random.NextDouble(), random.NextDouble(), random.NextDouble(), random.NextDouble());
        private static Station InitStation(int i, Random random) => new(i, StationNames[random.Next() % StationNames.Count], 0, 0, random.Next() % Station.MaxChargingPorts);
        private static Drone InitDrone(int i, Random random) => new(i, DroneModels[random.Next() % DroneModels.Count], random.NextDouble(), (WeightGroup)(random.Next()), (DroneStates)(i % 3));
        private static Customer InitCustumer(int i, Random random) => new(i, CustomerNames[random.Next() % CustomerNames.Count], GeneratePhone(), random.NextDouble(), random.NextDouble());

        private static string GeneratePhone()
        {
            Random random = new Random();
            return $"+972-5{random.Next() % 10}{random.Next(1000000, 9999999)}";
        }
    }
}

