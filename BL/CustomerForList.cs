using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class CustomerForList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberOfSentPackagesAccepted { get; set; }
        public int NumberOfSentPackagesOnTheWay { get; set; }
        public int NumberPackagesAccepted { get; set; }
        public int NumberPackagesOnTheWay { get; set; }

    }
}
