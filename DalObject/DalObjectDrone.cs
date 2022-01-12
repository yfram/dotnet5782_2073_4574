// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public partial class DalObject
    {
        /// <summary>
        /// Add a new drone to the data drone
        /// </summary>
        /// <param name="id">Id for new drone</param>
        /// <param name="model">Model for new drone</param>
        /// <param name="weight">Weight group for the new drone</param>
        /// <exception cref="ArgumentException"></exception>
        public void AddDrone(int id, string model, WeightGroup weight)
        {
            DataSource.Drones.Add(new(GetDroneIndex(id) != -1 ? throw new ArgumentException($"The drone {id} already exists") : id, model, weight));
        }

        /// <summary>
        /// Gets the drone with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted drone</param>
        /// <returns>The drone with id <paramref name="id"/></returns>
        /// <exception cref="ArgumentException"></exception>
        private static int GetDroneIndex(int id)
        {
            return DataSource.Drones.FindIndex(d => d.Id == id);
        }

        /// <summary>
        /// Get the index of the drone with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted drone</param>
        /// <returns>The index of the drone with id <paramref name="id"/></returns>
        public Drone GetDrone(int id)
        {
            int ix = GetDroneIndex(id);
            return ix == -1 ? throw new ArgumentException($"the drone {id} does not exist!") : DataSource.Drones[ix];
        }

        /// <summary>
        /// Gets all drones in the data drone that answer to predicate
        /// </summary>
        /// <param name="predicate">The function to filter drones with</param>
        /// <returns>A IEnumerable with all the drones in the data drone that answer to predicate</returns>
        public IEnumerable<Drone> GetAllDrones()
        {
            return new List<Drone>(DataSource.Drones);
        }

        /// <summary>
        /// Gets all drones in the data drone that answer to predicate
        /// </summary>
        /// <param name="predicate">The function to filter drones with</param>
        /// <returns>A IEnumerable with all the drones in the data drone that answer to predicate</returns>
        public IEnumerable<Drone> GetAllDronesWhere(Func<Drone, bool> predicate)
        {
            return new List<Drone>(DataSource.Drones.Where(predicate));
        }
    }
}
