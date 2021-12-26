using System.Collections.Generic;

namespace BO
{
    public class Station
    {
        public Station(int id, string name, Location locationOfStation, int amountOfEmptyPorts)
        {
            Id = id;
            Name = name;
            LocationOfStation = locationOfStation;
            AmountOfEmptyPorts = amountOfEmptyPorts;
            ChargingDrones = new List<DroneInCharging>();
        }

        public Station(int id, string name, Location locationOfStation, int amountOfEmptyPorts, List<DroneInCharging> chargingDrones)
        {
            Id = id;
            Name = name;
            LocationOfStation = locationOfStation;
            AmountOfEmptyPorts = amountOfEmptyPorts;
            ChargingDrones = chargingDrones;
        }
        public Station() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public Location LocationOfStation { get; set; }
        public int AmountOfEmptyPorts { get; set; }
        public IEnumerable<DroneInCharging> ChargingDrones { get; set; } = new List<DroneInCharging>();

        public override string ToString() => $"Id: {Id}\nName: {Name}\nAt: \n{LocationOfStation}";
    }
}
