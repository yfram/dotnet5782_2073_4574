namespace IDAL.DO
{
    public struct Drone
    {
        public Drone(int id, string model, double charge, WeightGroup weight, DroneStates state)
        {
            Id = id;
            Model = model;
            Weight = weight;
            State = state;
            Charge = charge;
        }

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
}