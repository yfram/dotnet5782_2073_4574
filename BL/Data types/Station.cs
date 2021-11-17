using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Station
    {
        public Station(int id, string name, Location locationOfStation, int amountOfEmptyPorts)
        {
            Id = id;
            Name = name;
            LocationOfStation = locationOfStation;
            AmountOfEmptyPorts = amountOfEmptyPorts;
            ChargingDrones = new();
        }

        public Station(int id, string name, Location locationOfStation, int amountOfEmptyPorts, List<DroneInCharging> chargingDrones)
        {
            Id = id;
            Name = name;
            LocationOfStation = locationOfStation;
            AmountOfEmptyPorts = amountOfEmptyPorts;
            ChargingDrones = chargingDrones;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public Location LocationOfStation { get; set; }
        public int AmountOfEmptyPorts { get; set; }
        public List<DroneInCharging> ChargingDrones { get; set; } = new();
    }
}
