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
        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots)
        {
            DataSource.Stations.Add(new(GetStationIndex(id) != -1 ? throw new ArgumentException($"the Station {id} already exists!") : id, name, longitude, lattitude, chargeSlots));
        }

        public Station GetStation(int id)
        {
            int ix = GetStationIndex(id);
            return ix == -1 ? throw new ArgumentException($"The Station {id} does not exist!") : DataSource.Stations[ix];
        }
        private static int GetStationIndex(int id)
        {
            return DataSource.Stations.FindIndex(s => s.Id == id);
        }

        public IEnumerable<Station> GetAllStations()
        {
            return new List<Station>(DataSource.Stations);
        }

        public IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate)
        {
            return DataSource.Stations.Where(predicate);
        }
    }
}