// File CustomerForList.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace BO
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

        public CustomerForList() { }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int NumberOfSentPackagesAccepted { get; set; }
        public int NumberOfSentPackagesOnTheWay { get; set; }
        public int NumberPackagesAccepted { get; set; }
        public int NumberPackagesOnTheWay { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name}\n Phone: {PhoneNumber}\nNumber of packages sent: " +
$"{NumberOfSentPackagesAccepted + NumberOfSentPackagesOnTheWay}\nNumber of received packages: " +
$"{NumberPackagesAccepted}\nNumber of packages on the way: {NumberPackagesOnTheWay}";
        }
    }
}
