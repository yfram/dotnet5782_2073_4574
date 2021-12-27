using DO;
using System;
using System.Collections.Generic;

namespace DalObject
{
    public partial class DalObject
    {

        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude) =>
            DataSource.Customers.Add(new(GetCustomerIndex(id) != -1 ? throw new ArgumentException($"cannot create the customer {id} .it is already exist!") : id, name, phone, lattitude, longitude));

        public Customer GetCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            if (ix == -1)
                throw new ArgumentException($"the customer {id} does not exist!");
            return DataSource.Customers[ix];
        }

        private int GetCustomerIndex(int id) => DataSource.Customers.FindIndex(c => c.Id == id);

        public IEnumerable<Customer> GetAllCustomers() => new List<Customer>(DataSource.Customers);


    }
}
