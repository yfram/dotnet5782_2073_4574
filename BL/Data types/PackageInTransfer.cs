// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace BO
{
    public class PackageInTransfer
    {
        public PackageInTransfer(int id, bool inDelivery, WeightGroup weight, PriorityGroup priority, CustomerForPackage sender, CustomerForPackage reciver, Location pickUpLocation, Location dropOffLocation, double distance)
        {
            Id = id;
            InDelivery = inDelivery;
            Weight = weight;
            Priority = priority;
            Sender = sender;
            Reciver = reciver;
            PickUpLocation = pickUpLocation;
            DropOffLocation = dropOffLocation;
            this.Distance = distance;
        }

        public PackageInTransfer()
        {
        }

        public int Id { get; set; }
        public bool InDelivery { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Reciver { get; set; }
        public Location PickUpLocation { get; set; }
        public Location DropOffLocation { get; set; }
        public double Distance { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nSent By: {Sender.Name}\nTo: {Reciver.Name}\nPriority: {Priority}";
        }
    }
}
