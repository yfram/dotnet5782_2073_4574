using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class CustomerForList
    {
        public CustomerForList(int id, string name, string phoneNumber, int numberOfSentPackagesAccepted, int numberOfSentPackagesOnTheWay, int numberPackagesAccepted, int numberPackagesOnTheWay)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            NumberOfSentPackagesAccepted = numberOfSentPackagesAccepted;
            NumberOfSentPackagesOnTheWay = numberOfSentPackagesOnTheWay;
            NumberPackagesAccepted = numberPackagesAccepted;
            NumberPackagesOnTheWay = numberPackagesOnTheWay;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberOfSentPackagesAccepted { get; set; }
        public int NumberOfSentPackagesOnTheWay { get; set; }
        public int NumberPackagesAccepted { get; set; }
        public int NumberPackagesOnTheWay { get; set; }

        public override string ToString() => $"Id: {Id}\n Name: {Name}\n Phone: {PhoneNumber}\nNumber of packages sent: " +
            $"{NumberOfSentPackagesAccepted + NumberOfSentPackagesOnTheWay}\nNumber of recived packages: " +
            $"{NumberPackagesAccepted}\nNumber of packages on the way: {NumberPackagesOnTheWay}";
    }
}
