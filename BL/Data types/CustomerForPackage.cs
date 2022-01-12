namespace BO
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

        public override string ToString()
        {
            return $"Id: {Id}\n Name: {Name}";
        }
    }
}
