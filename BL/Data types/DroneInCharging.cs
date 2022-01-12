// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;

namespace BO
{
    public class DroneInCharging
    {
        public DroneInCharging(int id, double battery)
        {
            Id = id;
            Battery = battery;
        }

        public DroneInCharging()
        {
        }

        public int Id { get; set; }
        public double Battery { get; set; }
        public DateTime TimeEnterd { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nCharge: {Battery}";
        }
    }
}
