using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Dal
{
    public partial class DalXml
    {

        public void AddCustomer(int id, string name, string phone, double lattitude, double longitude)
        {
            throw new NotImplementedException();
        }

        public void DeleteCustomer(int id)
        {
            throw new NotImplementedException();
        }

        public void UpdateCustomer(Customer c)
        {
            throw new NotImplementedException();
        }

        public Customer GetCustomer(int id)
        {
            Customer? ans = GetAllCustomers().FirstOrDefault(c => c.Id == id);
            if(!ans.HasValue) throw new ArgumentException($"the customer {id} does not exist!");
            return (Customer)ans;
        }



    }
}
