// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;

namespace DO
{
    public struct DroneCharge
    {
        public DroneCharge(int droneId, int stationId, DateTime time)
        {
            DroneId = droneId;
            StationId = stationId;
            Enter = time;

        }

        public int DroneId { get; set; }
        public int StationId { get; set; }
        public DateTime Enter { get; set; }

        public override string ToString()
        {
            return $"Drone id: {DroneId}\n Station id: {StationId}\n";
        }

    }
}