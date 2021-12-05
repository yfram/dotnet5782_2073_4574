using System.Collections.Generic;

namespace IBL.BO
{
    public class Customer
    {
        public Customer(int id, string name, string phoneNumber, Location customerLocation, List<PackageForCustomer> packagesFrom, List<PackageForCustomer> packagesTo)
        {
            Id = id;
            Name = name;
            PhoneNumber = phoneNumber;
            CustomerLocation = customerLocation;
            PackagesFrom = packagesFrom;
            PackagesTo = packagesTo;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location CustomerLocation { get; set; }
        public List<PackageForCustomer> PackagesFrom { get; set; }
        public List<PackageForCustomer> PackagesTo { get; set; }

        public override string ToString()
        => $"Id: {Id}\n Name: {Name}\n Phone: {PhoneNumber}\nLocated At: {CustomerLocation}";
    }
}
