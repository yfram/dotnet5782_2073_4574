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
        public void AddStation(int id, string name, double longitude, double lattitude, int chargeSlots) =>
            DataSource.Stations.Add(new(GetStationIndex(id) != -1 ? throw new Exception($"the Station {id} is already exist!") : id, name, longitude, lattitude, chargeSlots));
        public Station GetStation(int id)
        {
            int ix = GetStationIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the Station {id} is not exist!");
            return DataSource.Stations[ix];
        }
        private int GetStationIndex(int id) => DataSource.Stations.FindIndex(s => s.Id == id);

        public IEnumerable<Station> GetAllStations() => new List<Station>(DataSource.Stations);

        public IEnumerable<Station> GetAllAvailableStations() => DataSource.Stations.Where(p => p.ChargeSlots > 0);

    }
}
