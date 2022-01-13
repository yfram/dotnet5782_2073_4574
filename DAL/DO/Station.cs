// File Station.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace DO
{
    public struct Station
    {
        public Station(int id, string name, double longitude, double latitude, int chargeSlots)
        {
            Id = id;
            Name = name;
            Longitude = longitude;
            Latitude = latitude;
            ChargeSlots = chargeSlots;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public int ChargeSlots { get; set; }

        public override string ToString()
        {
            return $"id: {Id}\n name: {Name}\n Longitude: {Longitude}\n Latitude: {Latitude}\n ChargeSlots: {ChargeSlots}";
        }
    }
}