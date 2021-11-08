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
            DataSource.Stations.Add(new(id, name, longitude, lattitude, chargeSlots));
        public string GetStationString(int id) => DataSource.Stations[GetStationIndex(id)].ToString();
        private int GetStationIndex(int id) => DataSource.Stations.FindIndex(s => s.Id == id);

        public IEnumerable<Station> GetAllStations() => new List<Station>(DataSource.Stations);

        public IEnumerable<Station> GetAllAvailableStations() => DataSource.Stations.Where(p => p.ChargeSlots > 0);

    }
}
