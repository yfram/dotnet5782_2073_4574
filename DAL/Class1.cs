using System;

namespace DAL
{
    namespace DO
    {
        public struct Station
        {
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

        public struct Customer
        {
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
}
