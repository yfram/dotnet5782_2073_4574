using System;

namespace DAL.DO
{
    public enum WeightGroup { Light, Mid, Heavy }
    public enum DroneStates { Light, Mid, Heavy }
    public enum Priority { Low, Medium, High }

    public struct Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public double Charge
        {
            get => Charge;
            set
            {
                Charge = value < 100 && value > 0 ? value : Charge;
            }
        }
        public WeightGroup Weight { get; set; }
        public DroneStates State { get; set; }

        public override string ToString()
        {
            return $"Drone {Id}(Model num:{Model})";
        }
    }

    public struct Package
    {
        public int Id {  get; set; }
        public string SenderId { get; set; }
        public string RecevirId { get; set; }
        public WeightGroup Weight {  get; set; }
        public Priority PackagePriority {  get; set; }
        public int DroneId { get; set; }
        public double TimeToPackage {  get; set; }
        public double TimeToGetDrone { get; set; }
        public double TimeToGetPackedge { get; set; }
        public double TimeToRecive {  get; set; }
    }
}
