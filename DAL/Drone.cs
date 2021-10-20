namespace IDAL.DO
{
    public struct Drone
    {
        public Drone(int id, string model, double _charge, WeightGroup weight, DroneStates state)
        {
            Id = id;
            Model = model;
            Weight = weight;
            State = state;
            charge = _charge;
        }

        public int Id { get; set; }
        public string Model { get; set; }
        public double Charge
        {
            get => charge;
            set
            {
                charge = value < 100 && value > 0 ? value : charge;
            }
        }
        public WeightGroup Weight { get; set; }
        public DroneStates State { get; set; }

        private double charge;

        public override string ToString()
        {
            return $"Drone {Id}(Model num:{Model})";
        }
    }
}