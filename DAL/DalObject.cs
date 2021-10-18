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
        internal static Drone[] Drones = new Drone[10];
        internal static DroneStates[] States = new DroneStates[5];
        internal static Customer[] Customers = new Customer[100];
        internal static Package[] Packages = new Package[1000];

        internal class Config
        {
            internal static int DronesCounter = 0;
            internal static int StatesCounter = 0;
            internal static int CustomersCounter = 0;
            internal static int PackagesCounter = 0;
            internal static int RunNumber = 0;

        }

        public static void Initialize()
        {
            int droneNumber = 5;
            for (int i = 0; i < droneNumber; i++)
            {
                Drones[i] = initDrone();
            }


        }

        /*
         * needs fixing to actually work
          private static GetEnumer(Enum myEnum)
        {
            Array arr = Enum.GetValues(myEnum.GetType());
            Random r = new Random();
            return (myEnum.GetType())arr.GetValue(r.Next(0, arr.Length));
        }*/
        private static Drone initDrone()
        {
            return new Drone(Config.DronesCounter++, "TEST", 1, GetEnumer(WeightGroup), GetEnumer(DroneStates));
        }

        private static Customer initCustumer()
        {
            return new Customer();
        }
    }
}
}
