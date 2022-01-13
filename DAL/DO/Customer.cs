// File Customer.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace DO
{
    public struct Customer
    {
        public Customer(int id, string name, string phone, double latitude, double longitude)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Latitude = latitude;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name}\n Phone: {Phone}\nLongitude: {Longitude}\n Latitude: {Latitude}\n";
        }
    }
}