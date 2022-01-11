using BO;
using System;
using static System.Math;

namespace BLApi
{
    internal class LocationUtil
    {
        private static readonly int earthRadius = 6371;

        // https://www.igismap.com/formula-to-find-bearing-or-heading-angle-between-two-points-latitude-longitude/
        internal static double Bearing(Location source, Location destination)
        {

            Location radianSource = ToRadian(source);
            Location radianDestination = ToRadian(destination);

            double x = Cos(radianDestination.Latitude) * Sin((radianDestination.Longitude - radianSource.Longitude));

            double y = (Cos(radianSource.Latitude) * Sin(radianDestination.Latitude))
                - (Sin(radianSource.Latitude) *
                Cos(radianDestination.Latitude)
                * Cos(radianDestination.Longitude - radianSource.Longitude));

            double bearing = Atan2(x, y);

            bearing = ((bearing * 180 / PI + 360) % 360);
            return bearing;
        }

        internal static Location ToRadian(Location loc) => new Location(loc.Longitude * PI / 180, loc.Latitude * PI / 180);

        internal static double DistanceTo(Location Loc1, Location Loc2, char unit = 'K')
        {
            double lat1 = Loc1.Latitude; double lon1 = Loc1.Longitude;
            double lat2 = Loc2.Latitude; double lon2 = Loc2.Longitude;

            if (lat1 == lat2 && lon1 == lon2)
            {
                return 0;
            }

            double rlat1 = PI * lat1 / 180;
            double rlat2 = PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = PI * theta / 180;
            double dist =
                Sin(rlat1) * Sin(rlat2) + Cos(rlat1) *
                Cos(rlat2) * Cos(rtheta);
            dist = Acos(dist);
            dist = dist * 180 / PI;
            dist = dist * 60 * 1.1515;
            return unit switch
            {
                'K' => dist * 1.609344,
                'N' => dist * 0.8684,
                'M' => dist,
                _ => throw new NotSupportedException($"Unit type '{unit}' not supported yet"),
            };
        }

        internal static Location UpdateLocation(Location source, double progress, double bearing)
        {
            Location loc = new Location(0, 0);

            double ratio = progress / earthRadius;

            double bearingRad = bearing * PI / 180;
            Location sourceRad = ToRadian(source);

            loc.Latitude = Asin(
                Sin(sourceRad.Latitude) * Cos(ratio)
                +
                Cos(sourceRad.Latitude) * Sin(ratio) * Cos(bearingRad));

            loc.Latitude = loc.Latitude * 180 / PI;

            loc.Longitude = source.Longitude + Atan2(Sin(bearingRad) * Sin(ratio) * Cos(sourceRad.Latitude)
                ,
                Cos(ratio) - Sin(sourceRad.Latitude) * Sin(loc.Latitude * PI / 180)) * 180 / PI;

            loc.Longitude = (loc.Longitude + 540) % 360 - 180;

            return loc;
        }

        internal static bool IsNear(Location a, Location b) => DistanceTo(a, b) < 2;


    }
}
