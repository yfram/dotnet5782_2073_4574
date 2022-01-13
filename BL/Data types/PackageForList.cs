// File PackageForList.cs created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace BO
{
    public class PackageForList
    {
        public PackageForList(int id, string nameOfSender, string nameOfReciver, WeightGroup weight, PriorityGroup priority, PackageStatus status)
        {
            Id = id;
            NameOfSender = nameOfSender;
            NameOfReciver = nameOfReciver;
            Weight = weight;
            Priority = priority;
            Status = status;
        }

        public PackageForList() { }

        public int Id { get; set; }
        public string NameOfSender { get; set; }
        public string NameOfReciver { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public PackageStatus Status { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}\nSent by: {NameOfSender}\nTo: {NameOfReciver}\nPriority: {Priority}\nStatus: {Status}";
        }
    }
}
