using DO;
using System;
using System.Collections.Generic;

namespace DalObject
{
    public partial class DalObject
    {
        public void AddDrone(int id, string model, WeightGroup weight) =>
            DataSource.Drones.Add(new(GetDroneIndex(id) != -1 ? throw new ArgumentException($"the drone {id} already exists") : id, model, weight));

        public IEnumerable<Drone> GetAllDrones() => new List<Drone>(DataSource.Drones);

        private int GetDroneIndex(int id) => DataSource.Drones.FindIndex(d => d.Id == id);
        public Drone GetDrone(int id)
        {
            int ix = GetDroneIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the drone {id} does not exist!");
            return DataSource.Drones[ix];
        }


    }
}
