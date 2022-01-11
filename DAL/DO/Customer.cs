namespace DO
{
    public struct Customer
    {
        public Customer(int id, string name, string phone, double lattitude, double longitude)
        {
            Id = id;
            Name = name;
            Phone = phone;
            Lattitude = lattitude;
            Longitude = longitude;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Lattitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name}\n Phone: {Phone}\nLongitude: {Longitude}\n Latitude: {Lattitude}\n";
        }
    }
}