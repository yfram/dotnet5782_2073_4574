using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public class DataSource
    {

        internal class Config
        {
            internal static double ElecEmpty = 0;
            internal static double ElecLow = 0;
            internal static double ElecMid = 0;
            internal static double ElecHigh = 0;
            internal static double ElecRatePercent = 0;
            internal static int RunNumber = 0;
            internal static int MaxChargingPorts = 10;
        }

        #region Sample data
        private static readonly List<string> DroneModels = new List<string>()
        {
            "DJIM1", "DJIF3", "Mavic Pro", "DJI Phantom 4","DJI Mavic 2 Zoom", "DJI Mavic 2 Pro"
        };
        private static readonly List<string> StationNames = new List<string>()
        {
            "Tel Aviv", "Bear Sheava", "Modi'in", "Jeruselam","Heifa"
        };
        private static readonly List<string> CustomerNames = new List<string>()
        {
            "Yoni", "Gil", "Guy","Gal","Pete","Winston","Nick","Neo","Morpheus", "Trinity"
        };
        #endregion

        internal static List<Drone> Drones { get; set; } = new(10);
        internal static List<Station> Stations { get; set; } = new(5);
        internal static List<Customer> Customers { get; set; } = new(100);
        internal static List<Package> Packages { get; set; } = new(1000);
        internal static List<DroneCharge> DroneCharges { get; set; } = new();
        internal static Config Configuration { get; set; } = new();

        public static void Initialize()
        {
            Random random = new Random();

            double elec = random.NextDouble() + 40;
            Config.ElecEmpty = elec;
            Config.ElecLow = elec / 2;
            Config.ElecMid = elec / 4;
            Config.ElecHigh = elec / 8;
            Config.ElecRatePercent = random.NextDouble() * 2 + 10;

            for (int i = 0; i < 15; i++)
            {
                Stations.Add(InitStation(i, random));
            }

            for (int i = 0; i < 5; i++)
            {
                Drones.Add(InitDrone(i, random));
            }

            for (int i = 0; i < 10; i++)
            {
                Customers.Add(InitCustumer(i, random));
            }

            for (int i = 0; i < 40; i++)
            {
                Packages.Add(InitPackage(random));
            }

            Packages.Sort(Comparer<Package>.Create((i1, i2) => i1.PackagePriority.CompareTo(i2.PackagePriority)));

            List<Drone> dronesWithoutPackages = Drones.ToList();

            for (int i = 0; i < Packages.Count; i++)
            {
                Package temp = Packages[i];
                temp.DroneId = null;
                temp.Created = DateTime.Now.AddHours(random.Next(0, 500));//gives a good range of times
                int state = random.Next(0, 5);
                bool hasDrone = dronesWithoutPackages.Exists(d => d.Weight >= temp.Weight);
                if (state != 0 && hasDrone)
                {
                    if (state > 0) // Associated
                    {
                        temp.Associated = ((DateTime)temp.Created).AddMinutes(random.Next(1, 3000));
                    }

                    if (state > 1) // PickUp
                    {
                        temp.PickUp = ((DateTime)temp.Associated).AddMinutes(random.Next(1, 3000));
                    }

                    if (state > 2) // Delivered
                    {
                        temp.Delivered = ((DateTime)temp.PickUp).AddMinutes(random.Next(1, 3000));
                    }

                    Drone d = dronesWithoutPackages.Find(d => d.Weight >= temp.Weight);
                    if (state != 4)
                    {
                        dronesWithoutPackages.Remove(d); // if the state is "delivered" so the drone hasn't a package now.
                    }

                    temp.DroneId = d.Id;
                }
                Packages[i] = temp;
            }
        }

        #region Init functions
        private static Package InitPackage(Random random)
        {
            int reciver = random.Next() % 10;
            int i = Config.RunNumber;
            Config.RunNumber++;
            return new(i, Customers[i % 10].Id, Customers[reciver == i % 10 ? ((i + 1) % Customers.Count) : reciver].Id,
                (WeightGroup)(random.Next() % 3 + 1), (Priority)(i % 3 + 1), null);

        }
        private static Station InitStation(int i, Random random) =>
            new(i, StationNames[random.Next() % StationNames.Count], 33 + random.NextDouble(), 34 + random.NextDouble(),
                random.Next() % Config.MaxChargingPorts);
        private static Drone InitDrone(int i, Random random) =>
            new(i, DroneModels[random.Next() % DroneModels.Count], (WeightGroup)(random.Next(1, 4)));
        private static Customer InitCustumer(int i, Random random) =>
            new(i, CustomerNames[random.Next() % CustomerNames.Count],
                $"+972-5{random.Next() % 10}{random.Next(1000000, 9999999)}", 33 + random.NextDouble(), 34 + random.NextDouble());
        #endregion
    }
}

