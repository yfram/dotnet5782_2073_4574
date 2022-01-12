﻿using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    public partial class DalObject
    {

        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude)
        {
            DataSource.Customers.Add(new(GetCustomerIndex(id) != -1 ? throw new ArgumentException($"Cannot create the customer {id}. it is already exist!") : id, name, phone, lattitude, longitude));
        }

        public Customer GetCustomer(int id)
        {
            int ix = GetCustomerIndex(id);
            return ix == -1 ? throw new ArgumentException($"the customer {id} does not exist!") : DataSource.Customers[ix];
        }

        private static int GetCustomerIndex(int id)
        {
            return DataSource.Customers.FindIndex(c => c.Id == id);
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            return new List<Customer>(DataSource.Customers);
        }

        public IEnumerable<Customer> GetAllCustomerssWhere(Func<Customer, bool> predicate)
        {
            return new List<Customer>(DataSource.Customers.Where(predicate));
        }
    }
}
