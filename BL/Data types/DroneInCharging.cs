using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class DroneInCharging
    {
        public DroneInCharging(int id, double battery)
        {
            Id = id;
            Battery = battery;
        }

        public DroneInCharging()
        {
        }

        public int Id { get; set; }
        public double Battery { get; set; }

        public override string ToString() => $"Id: {Id}\nCharge: {Battery}";
    }
}
