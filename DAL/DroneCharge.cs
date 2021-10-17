namespace IDAL.DO
{
    public struct DroneCharge
    {
        public int DroneId { get; set; }
        public int StationId { get; set; }

        public override string ToString()
        {
            return $"Drone id: {DroneId}\n Station id: {StationId}\n";
        }


    }
}