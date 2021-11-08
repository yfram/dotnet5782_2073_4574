using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location LocationOfStation { get; set; }
        public int AmountOfEmptyPorts { get; set; }
        public List<DroneInCharging> ChargingDrones { get; set; } = new();
    }
}
