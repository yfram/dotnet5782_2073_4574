using System;

namespace IDAL.DO
{
    public enum WeightGroup { Light, Mid, Heavy }
    public enum DroneStates { Empty, Maintenance, Shipping }
    public enum Priority { Low, Medium, High }
}

    /*
    public struct Drone
    {
        public Drone(int id, string model, double charge, WeightGroup weight, DroneStates state)
        {
            Id = id;
            Model = model;
            Weight = weight;
            State = state;
            Charge = charge;
        }

        public int Id { get; set; }
        public string Model { get; set; }
        public double Charge
        {
            get => Charge;
            set
            {
                Charge = value < 100 && value > 0 ? value : Charge;
            }
        }
        public WeightGroup Weight { get; set; }
        public DroneStates State { get; set; }

        public override string ToString()
        {
            return $"Drone {Id}(Model num:{Model})";
        }
    }
    */

    /*
    public struct Package
    {
        public Package(int id, string senderId, string recevirId, WeightGroup weight, Priority packagePriority, int droneId, double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive)
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
            DroneId = droneId;
            TimeToPackage = timeToPackage;
            TimeToGetDrone = timeToGetDrone;
            TimeToGetPackedge = timeToGetPackedge;
            TimeToRecive = timeToRecive;
        }

        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecevirId { get; set; }
        public WeightGroup Weight { get; set; }
        public Priority PackagePriority { get; set; }
        public int DroneId { get; set; }
        public double TimeToPackage { get; set; }
        public double TimeToGetDrone { get; set; }
        public double TimeToGetPackedge { get; set; }
        public double TimeToRecive { get; set; }

        public override string ToString()
        {//TODO
            return $"Package {Id}";//TODO!  ;
        }
    }
    */


    /*
        public struct Station
        {
        public Station(int id, int name, double longitude, double lattitude, int chargeSlots)
        {
            Id = id;
            Name = name;
            Longitude = longitude;
            Lattitude = lattitude;
            ChargeSlots = chargeSlots;
        }

        public int Id { get; set; }
            public int Name { get; set; }
            public double Longitude { get; set; }
            public double Lattitude { get; set; }
            public int ChargeSlots { get; set; }

            public override string ToString()
            {
                return $"id: {Id}\n name: {Name}\n Longitude: {Longitude}\n Lattitude: {Lattitude}\n ChargeSlots: {ChargeSlots}";
            }
        }

    */

    /*
        public struct Customer
        {
        public Customer(int id, int name, string phone, double lattitude, double longitude)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Lattitude = lattitude;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public int Name { get; set; }
            public string Phone { get; set; }
            public double Lattitude { get; set; }
            public double Longitude { get; set; }

            public override string ToString()
            {
                return $"Id: {Id}\n Name: {Name}\n Phone: {Phone}\nLongitude: {Longitude}\n Lattitude: {Lattitude}\n";
            }
        }

        public struct DroneCharge
        {
            public int DroneId { get; set; }
            public int StationId { get; set; }

            public override string ToString()
            {
                return $"Drone id: {DroneId}\n Station id: {StationId}\n";
            }


        }
}

    */
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
        internal static Drone[] Drones = new DO.Drone[10];
        internal static DO.DroneStates[] States = new DO.DroneStates[5];
        internal static DO.Customer[] Customers = new DO.Customer[100];
        internal static DO.Package[] Packages = new DO.Package[1000];

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
            for(int i=0; i<droneNumber; i++)
            {
                Drones[i] = initDrone();
            }


        }

        private static GetEnumer(Enum myEnume)
        {
            Array arr = Enum.GetValues(typeof(myEnum));
            Random r = new Random();
            return (myEnum)arr.GetValue(r.Next(0, arr.Length));
        }
        private static IDAL.DO.Drone initDrone()
        {
            return new DO.Drone(Config.DronesCounter++, "TEST",1, GetEnumer(DO.WeightGroup), GetEnumer(DO.DroneStates));
        }

        private static DO.Customer initCustumer()
        {
            return new DO.Customer();
        }
    }
}
