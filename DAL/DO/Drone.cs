namespace DO
{
    public struct Drone
    {
        public Drone(int id, string model, WeightGroup weight)
        {
            Id = id;
            Model = model;
            Weight = weight;

        }

        public int Id { get; set; }
        public string Model { get; set; }

        public WeightGroup Weight { get; set; }

        public override string ToString()
        {
            return $"Drone {Id}(Model num:{Model})";
        }
    }
}