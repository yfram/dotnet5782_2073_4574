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
            DataSource.Customers.Add(new(id, name, phone, lattitude, longitude));

        public string GetCustomerString(int id) => DataSource.Customers[GetCustomerIndex(id)].ToString();

        public IEnumerable<Customer> GetAllCustomers() => DataSource.Customers;
    }
}
