using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Drone
    {
        public Drone(int id, string model, WeightGroup weight, double battery, DroneState state, PackageInTransfer package, Location currentLocation)
        {
            Id = id;
            Model = model;
            Weight = weight;
            Battery = battery;
            State = state;
            Package = package;
            CurrentLocation = currentLocation;
        }

        public Drone()
        {
        }

        public int Id { get; set; }
        public string Model { get; set; }
        public WeightGroup Weight { get; set; }
        public double Battery { get; set; }
        public DroneState State { get; set; }
        public PackageInTransfer Package { get; set; }
        public Location CurrentLocation { get; set; }

        public override string ToString() =>
#pragma warning disable CS8524
            $"Id: {Id}\nModel: {Model}\nCharge: {Battery}\nCurrently at: \n{CurrentLocation}\n" +
                $"State: {State}\n" + (Package is not null? $"Package Id: {Package.Id}":"");
#pragma warning restore CS8524

    }
}
