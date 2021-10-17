namespace IDAL.DO
{
    public struct Package
    {
        public Package(int id, string senderId, string recevirId, WeightGroup weight, Priority packagePriority, int droneId, double timeToPackage, double timeToGetDrone, double timeToGetPackedge, double timeToRecive)
        {
            Id = id;
            SenderId = senderId;
            RecevirId = recevirId;
            Weight = weight;
            PackagePriority = packagePriority;
            DroneId = droneId;
            TimeToPackage = timeToPackage;
            TimeToGetDrone = timeToGetDrone;
            TimeToGetPackedge = timeToGetPackedge;
            TimeToRecive = timeToRecive;
        }

        public int Id { get; set; }
        public string SenderId { get; set; }
        public string RecevirId { get; set; }
        public WeightGroup Weight { get; set; }
        public Priority PackagePriority { get; set; }
        public int DroneId { get; set; }
        public double TimeToPackage { get; set; }
        public double TimeToGetDrone { get; set; }
        public double TimeToGetPackedge { get; set; }
        public double TimeToRecive { get; set; }

        public override string ToString()
        {//TODO
            return $"Package {Id}";//TODO!  ;
        }
    }
}