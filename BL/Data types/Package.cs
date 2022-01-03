using System;

namespace BO
{
    public class Package
    {

        public Package(int id, CustomerForPackage sender, CustomerForPackage reciver, WeightGroup weight, PriorityGroup priority, DroneForPackage drone, DateTime? timeToPackage, DateTime? timeToPair, DateTime? timeToPickup, DateTime? timeToDeliver)
        {
            Id = id;
            Sender = sender;
            Reciver = reciver;
            Weight = weight;
            Priority = priority;
            Drone = drone;
            TimeToPackage = timeToPackage;
            TimeToPair = timeToPair;
            TimeToPickup = timeToPickup;
            TimeToDeliver = timeToDeliver;
        }

        public Package()
        {
        }

        public int Id { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Reciver { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public DroneForPackage Drone { get; set; }
        public DateTime? TimeToPackage { get; set; }
        public DateTime? TimeToPair { get; set; }
        public DateTime? TimeToPickup { get; set; }
        public DateTime? TimeToDeliver { get; set; }

#pragma warning disable CS8524 
        public override string ToString() => $"Id: {Id}\n Sender ID: {Sender.Id}\n Reciver ID: {Reciver.Id}\nPriority: " +
                $"{Priority}\n" + (Drone is not null ? $"Drone Id: {Drone.Id}" : "");
#pragma warning restore CS8524
    }
}
