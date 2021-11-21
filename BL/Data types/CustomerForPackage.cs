using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class CustomerForPackage
    {
        public CustomerForPackage(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public CustomerForPackage()
        {
        }

        public CustomerForPackage(Customer customer) : this(customer.Id, customer.Name) { }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}
