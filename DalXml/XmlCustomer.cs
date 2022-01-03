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
            var ans = GetAllCustomers();
            if (ans.Where(c=>c.Id==id).Count() > 0)
                throw new ArgumentException($"the customer {id} already exist");

            WriteAllCustomers(ans.Append(new Customer(id,name,phone,lattitude,longitude)));
        }

        public void DeleteCustomer(int id)
        {
            GetCustomer(id); // check if exist

            var ObjectsRoot = XElement.Load($"Data/Customers.xml");

            (from s in ObjectsRoot.Elements()
                       where Int32.Parse(s.Element("Id").Value) == id
                       select s).FirstOrDefault().Remove();

            ObjectsRoot.Save($"Data/Customers.xml");
        }

        public void UpdateCustomer(Customer c)
        {

            var ObjectsRoot = XElement.Load($"Data/Customers.xml");

            XElement e = (from s in ObjectsRoot.Elements()
                         where Int32.Parse(s.Element("Id").Value) == c.Id
                         select s).First();

            if(e is null)
                throw new ArgumentException($"the id {c.Id} is not exsist!");

            e.Element("Name").Value = c.Name;
            e.Element("Phone").Value = c.Phone;
            e.Element("Longitude").Value = c.Longitude.ToString();
            e.Element("Lattitude").Value = c.Lattitude.ToString();

            ObjectsRoot.Save($"Data/Customers.xml");
        }

        public Customer GetCustomer(int id)
        {
            var all = GetAllCustomers();
            foreach(var elem in all)
            {
                if (elem.Id == id)
                    return elem;
            }
            throw new ArgumentException($"the id {id} is not exist!");
        }

        public IEnumerable<Customer> GetAllCustomers()
        {
            var ObjectsRoot = XElement.Load($"Data/Customers.xml");

            return (from s in ObjectsRoot.Elements()
                    select new Customer()
                    {
                        Id = Int32.Parse(s.Element("Id").Value),
                        Name = s.Element("Name").Value,
                        Phone = s.Element("Phone").Value,
                        Lattitude = Double.Parse(s.Element("Lattitude").Value),
                        Longitude = Double.Parse(s.Element("Longitude").Value)
                    });
        }

        private void WriteAllCustomers(IEnumerable<Customer> write)
        {
            XElement root = new XElement("listOfObjects",
                from p in write
            select new XElement("Customer",
            new XElement("Id", p.Id),
            new XElement("Name",p.Name),
            new XElement("Phone",p.Phone),
            new XElement("Lattitude", p.Lattitude),
            new XElement("Longitude", p.Longitude)
            ));

            root.Save($"Data/Customers.xml");

        }

    }
}
