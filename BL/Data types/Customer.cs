using System.Collections.Generic;

namespace BO
{
    public class Customer
    {
        public Customer(int id, string name, string phoneNumber, Location customerLocation, IEnumerable<PackageForCustomer> packagesFrom, IEnumerable<PackageForCustomer> packagesTo)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            CustomerLocation = customerLocation;
            PackagesFrom = packagesFrom;
            PackagesTo = packagesTo;
        }
        public Customer() { }
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location CustomerLocation { get; set; }
        public IEnumerable<PackageForCustomer> PackagesFrom { get; set; }
        public IEnumerable<PackageForCustomer> PackagesTo { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name}\n Phone: {PhoneNumber}\nLocated At: {CustomerLocation}";
        }
    }
}
