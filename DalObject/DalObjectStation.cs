using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public partial class DalObject
    {
        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots) =>
            DataSource.Stations.Add(new(GetStationIndex(id) != -1 ? throw new ArgumentException($"the Station {id} already exists!") : id, name, longitude, lattitude, chargeSlots));
        public Station GetStation(int id)
        {
            int ix = GetStationIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Station {id} does not exist!");
            return DataSource.Stations[ix];
        }
        private int GetStationIndex(int id) => DataSource.Stations.FindIndex(s => s.Id == id);

        public IEnumerable<Station> GetAllStations() => new List<Station>(DataSource.Stations);

        public IEnumerable<Station> GetAllStationsWhere(Func<Station, bool> predicate) => DataSource.Stations.Where(predicate);

    }
}