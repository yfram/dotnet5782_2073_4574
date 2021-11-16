
using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Customer()
        {
        }

        public int Id {  get; set; }
        public string Name {  get; set; }
        public string PhoneNumber { get; set; }
        public Location CustomerLocation {  get; set; }
        public List<PackageForCustomer> PackagesFrom {  get; set; }
        public List<PackageForCustomer> PackagesTo {  get; set; }
    }
}
