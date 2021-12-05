namespace IBL.BO
{
    public class Location
    {
        public Location(double longitude, double latitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public double Longitude { get; set; }
        public double Latitude { get; set; }

        public override string ToString() => $"Longitude: {Longitude}\nLatitude: {Latitude}";
    }
}
