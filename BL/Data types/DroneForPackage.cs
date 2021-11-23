﻿namespace IBL.BO
{
    public class DroneForPackage
    {
        public DroneForPackage(int id, double battery, Location currentLocation)
        {
            Id = id;
            Battery = battery;
            CurrentLocation = currentLocation;
        }

        public DroneForPackage()
        {
        }

        public int Id { get; set; }
        public double Battery { get; set; }
        public Location CurrentLocation { get; set; }

        public override string ToString() => $"Id: {Id}\nCharge: {Battery}\nLocation: \n{CurrentLocation}";
    }
}