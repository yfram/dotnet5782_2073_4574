// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;

namespace BO
{
    public class Package
    {

        public Package(int id, CustomerForPackage sender, CustomerForPackage reciver, WeightGroup weight, PriorityGroup priority, DroneForPackage drone, DateTime? timePackaged, DateTime? timePaired, DateTime? timePickedUp, DateTime? timeDeliverd)
        {
            Id = id;
            Sender = sender;
            Reciver = reciver;
            Weight = weight;
            Priority = priority;
            Drone = drone;
            TimePackaged = timePackaged;
            TimePaired = timePaired;
            TimePickedUp = timePickedUp;
            TimeDeliverd = timeDeliverd;
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
        public override string ToString()
        {
            return $"Id: {Id}\n Sender ID: {Sender.Id}\n Receiver ID: {Reciver.Id}\nPriority: " +
                $"{Priority}\n" + (Drone is not null ? $"Drone Id: {Drone.Id}" : "");
        }
    }
}
