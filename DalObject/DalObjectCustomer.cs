// File DalObjectCustomer.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Dal
{
    public partial class DalObject
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude)
        {
            DataSource.Customers.Add(new(GetCustomerIndex(id) != -1 ? throw new ArgumentException($"Cannot create the customer {id}. it is already exist!") : id, name, phone, lattitude, longitude));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public Customer GetCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            return ix == -1 ? throw new ArgumentException($"the customer {id} does not exist!") : DataSource.Customers[ix];
        }

        private int GetCustomerIndex(int id)
        {
            return DataSource.Customers.FindIndex(c => c.Id == id);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetAllCustomers()
        {
            return new List<Customer>(DataSource.Customers);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IEnumerable<Customer> GetAllCustomerssWhere(Func<Customer, bool> predicate)
        {
            return new List<Customer>(DataSource.Customers.Where(predicate));
        }
    }
}
