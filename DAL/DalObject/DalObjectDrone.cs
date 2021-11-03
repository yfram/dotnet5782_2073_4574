using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalObject
{
    public partial class DalObject
    {
        public void AddDrone(int id, string model, WeightGroup weight) =>
            DataSource.Drones.Add(new(id, model, weight));

        public IEnumerable<Drone> GetAllDrones() => DataSource.Drones;

        public string GetDroneString(int id) => DataSource.Drones[GetDroneIndex(id)].ToString();

    }
}
