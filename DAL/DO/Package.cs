using System;

namespace DO
{
    public struct Package
    {
        public Package(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId) : this()
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
            DroneId = droneId;
        }

        public Package(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId, DateTime? created, DateTime? associated, DateTime? pickUp, DateTime? delivered)
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
            DroneId = droneId;
            Created = created;
            Associated = associated;
            PickUp = pickUp;
            Delivered = delivered;
        }

        public Package(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority) : this()
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
        }

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecevirId { get; set; }
        public WeightGroup Weight { get; set; }
        public Priority PackagePriority { get; set; }
        public int? DroneId { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Associated { get; set; }
        public DateTime? PickUp { get; set; }
        public DateTime? Delivered { get; set; }

        public override string ToString()
        {
            return $"Package {Id} to: {RecevirId} from:{SenderId}\nArriving in approx.." +
                $"{(DroneId is not null ? $"\nDroneId: {DroneId}" : "")}";
        }
    }
}