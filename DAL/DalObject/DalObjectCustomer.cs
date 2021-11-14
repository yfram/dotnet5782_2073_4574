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

        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude) =>
            DataSource.Customers.Add(new(GetCustomerIndex(id)!=-1? throw new ArgumentException($"cannot create the customer {id} .it is already exsist!") : id, name, phone, lattitude, longitude));

        public Customer GetCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the customer {id} is not exsist!");
            return DataSource.Customers[id];
        }

        private int GetCustomerIndex(int id) => DataSource.Customers.FindIndex(c => c.Id == id);

        public IEnumerable<Customer> GetAllCustomers() => new List<Customer>(DataSource.Customers);

        
    }
}
