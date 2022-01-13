// File DalObjectStation.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dal
{
    public partial class DalObject
    {
        /// <summary>
        /// Add a new station to the data base
        /// </summary>
        /// <param name="id">The id for the new station</param>
        /// <param name="name">The name for the new station</param>
        /// <param name="longitude">Longitude for the new station</param>
        /// <param name="latitude">Latitude for the new station</param>
        /// <param name="chargeSlots">Amount of charging ports in the new station</param>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddStation(int id, string name, double longitude, double latitude, int chargeSlots)
        {
            DataSource.Stations.Add(new(GetStationIndex(id) != -1 ? throw new ArgumentException($"the Station {id} already exists!") : id, name, longitude, latitude, chargeSlots));
        }

        /// <summary>
        /// Gets the station with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted station</param>
        /// <returns>The station with id <paramref name="id"/></returns>
        /// <exception cref="ArgumentException"></exception>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public Station GetStation(int id)
        {
            int ix = GetStationIndex(id);
            return ix == -1 ? throw new ArgumentException($"The Station {id} does not exist!") : DataSource.Stations[ix];
        }

        /// <summary>
        /// Get the index of the station with id <paramref name="id"/>
        /// </summary>
        /// <param name="id">The id for the wanted station</param>
        /// <returns>The index of the station with id <paramref name="id"/></returns>
        private int GetStationIndex(int id)
        {
            return DataSource.Stations.FindIndex(s => s.Id == id);
        }

        /// <summary>
        /// Gets all stations in the data base
        /// </summary>
        /// <returns>A IEnumerable with all the stations in the data base</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetAllStations()
        {
            return new List<Station>(DataSource.Stations);
        }

        /// <summary>
        /// Gets all stations in the data base that answer to <paramref name="predicate"/>
        /// </summary>
        /// <param name="predicate">The function to filter stations with</param>
        /// <returns>A IEnumerable with all the stations in the data base that answer to predicate</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate)
        {
            return DataSource.Stations.Where(predicate);
        }
    }
}