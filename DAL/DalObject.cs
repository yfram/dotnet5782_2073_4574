using System;
using IDAL.DO;

namespace DalObject
{
    public class DalObject
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        public void AddDroneStation(int id, string model, double charge, WeightGroup weight, DroneStates state)
        {

        }
    }
    public class DataSource
    {

        internal class Config
        {
            internal static int FirstDrone = 0;
            internal static int FirstStation = 0;
            internal static int RunNumber = 0;
        }

        internal static Drone[] Drones = new Drone[10];
        internal static Station[] Stations = new Station[5];
        internal static Customer[] Customers = new Customer[100];
        internal static Package[] Packages = new Package[1000];
        internal static Config Configuration { get; set; }

        public static void Initialize()
        {
            Configuration = new Config();  //Config is all 0's anyways
            Random random = new Random();
            for (int i = 0; i < 2; i++)
                Stations[i] = InitStation(i, random);
            for (int i = 0; i < 5; i++)
                Drones[i] = InitDrone(i, random);
            for (int i = 0; i < 10; i++)
                Customers[i] = InitCustumer(i, random);
            for (int i = 0; i < 10; i++)
                Packages[i] = InitPackage(i, random);
        }

        private static Package InitPackage(int i, Random random) => new(i, Customers[i].Id, Customers[(i + 11) % 10].Id, (WeightGroup)random.Next(), (Priority)(i % 3),
            0, 0, 0, 0, 0);
        private static Station InitStation(int i, Random random) => new(i, $"Station {i}", 0, 0, random.Next() % Station.MaxChargingPorts);
        private static Drone InitDrone(int i, Random random) => new(i, "TBD", random.NextDouble(), (WeightGroup)(random.Next()), (DroneStates)(i % 3));
        private static Customer InitCustumer(int i, Random random) => new(i, $"Person #{i}", GeneratePhone(), random.NextDouble(), random.NextDouble());

        private static string GeneratePhone()
        {
            Random random = new Random();
            return $"+972-5{random.Next() % 10}{random.Next(1000000, 9999999)}";
        }
    }
}
