// File DroneForList.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace BO
{
    public class DroneForList
    {
        public DroneForList(int id, string model, WeightGroup weight, double battery, DroneState state, Location currentLocation, int passingPackage)
        {
            Id = id;
            Model = model;
            Weight = weight;
            Battery = battery;
            State = state;
            CurrentLocation = currentLocation;
            PassingPckageId = passingPackage;
        }

        public DroneForList()
        {
        }

        public int Id { get; set; }
        public string Model { get; set; }
        public WeightGroup Weight { get; set; }
        public double Battery { get; set; }
        public DroneState State { get; set; }
        public Location CurrentLocation { get; set; }
        public int? PassingPckageId { get; set; } = null;

        public override string ToString()
        {
            return $"Id: {Id}\nModel: {Model}\nCharge: {Battery}\nCurrently at: {CurrentLocation.ToString().Replace("\n", "\t")}\n" +
$"State: {State}\n{(PassingPckageId is null ? "" : $"Package Id: {(PassingPckageId == -1 ? "None" : PassingPckageId)}") }";
        }
    }
}
