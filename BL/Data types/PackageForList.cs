using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
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

        public int Id { get; set; }
        public string NameOfSender { get; set; }
        public string NameOfReciver { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public PackageStatus Status { get; set; }
    }
}
