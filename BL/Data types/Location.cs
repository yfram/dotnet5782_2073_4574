using CoordinateSharp;

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

        public override string ToString()
        {
            Coordinate c = new Coordinate(Latitude,Longitude);// $"Longitude: {Longitude}\nLatitude: {Latitude}";
            c.FormatOptions.Format = CoordinateFormatType.Degree_Minutes_Seconds;
            return c.ToString();
        }

    }
}
