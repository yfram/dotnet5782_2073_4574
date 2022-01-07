using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public partial class DalObject
    {
        public void AddDrone(int id, string model, WeightGroup weight) =>
            DataSource.Drones.Add(new(GetDroneIndex(id) != -1 ? throw new ArgumentException($"The drone {id} already exists") : id, model, weight));

        private int GetDroneIndex(int id) => DataSource.Drones.FindIndex(d => d.Id == id);
        public Drone GetDrone(int id)
        {
            int ix = GetDroneIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the drone {id} does not exist!");
            return DataSource.Drones[ix];
        }

        public IEnumerable<Drone> GetAllDrones() => new List<Drone>(DataSource.Drones);
        public IEnumerable<Drone> GetAllDronesWhere(Func<Drone, bool> predicate) =>
            new List<Drone>(DataSource.Drones.Where(predicate));

    }
}
