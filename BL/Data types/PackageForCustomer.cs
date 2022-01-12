namespace BO
{
    public class PackageForCustomer
    {
        public PackageForCustomer(int id, WeightGroup weight, PriorityGroup priority, PackageStatus status, CustomerForPackage customer)
        {
            Id = id;
            Weight = weight;
            Priority = priority;
            Status = status;
            Customer = customer;
        }

        public PackageForCustomer()
        {
        }

        public int Id { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public PackageStatus Status { get; set; }
        public CustomerForPackage Customer { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nPriority: {Priority}" +
$"\nSending to: {Customer.Name}";
        }
    }
}
