namespace IDAL.DO
{
    public struct Package
    {
        public Package(int id, int senderId, int recevirId, WeightGroup weight, Priority packagePriority, int? droneId, double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive)
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
            DroneId = droneId;
            TimeToPackage = timeToPackage;
            TimeToGetDrone = timeToGetDrone;
            TimeToGetPackage = timeToGetPackedge;
            TimeToRecive = timeToRecive;
        }

        public int Id { get; set; }
        public int SenderId { get; set; }
        public int RecevirId { get; set; }
        public WeightGroup Weight { get; set; }
        public Priority PackagePriority { get; set; }
        public int? DroneId { get; set; }
        public double TimeToPackage { get; set; }
        public double TimeToGetDrone { get; set; }
        public double TimeToGetPackage { get; set; }
        public double TimeToRecive { get; set; }

        public override string ToString()
        {
            return $"Package {Id} to: {RecevirId} from:{SenderId}\nArriving in aprox. {TimeToPackage + TimeToGetPackage + TimeToGetDrone + TimeToRecive}";
        }
    }
}