using System;

namespace BO
{
    public class Package
    {

        public Package(int id, CustomerForPackage sender, CustomerForPackage reciver, WeightGroup weight, PriorityGroup priority, DroneForPackage drone, DateTime? TimePackaged, DateTime? TimePaired, DateTime? TimePickedUp, DateTime? TimeDeliverd)
        {
            Id = id;
            Sender = sender;
            Reciver = reciver;
            Weight = weight;
            Priority = priority;
            Drone = drone;
            TimePackaged = TimePackaged;
            TimePaired = TimePaired;
            TimePickedUp = TimePickedUp;
            TimeDeliverd = TimeDeliverd;
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
        public DateTime? TimePackaged { get; set; }
        public DateTime? TimePaired { get; set; }
        public DateTime? TimePickedUp { get; set; }
        public DateTime? TimeDeliverd { get; set; }

#pragma warning disable CS8524 
        public override string ToString() => $"Id: {Id}\n Sender ID: {Sender.Id}\n Receiver ID: {Reciver.Id}\nPriority: " +
                $"{Priority}\n" + (Drone is not null ? $"Drone Id: {Drone.Id}" : "");
#pragma warning restore CS8524
    }
}
