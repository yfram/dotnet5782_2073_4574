using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Package
    {
        public int Id { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Reciver { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public DroneForPackage Drone { get; set; }
        ///<summary>
        ///Time to package in minutes
        ///</summary>
        public double TimeToPackage { get; set; }
        ///<summary>
        ///Time to pair in minutes
        ///</summary>
        public double TimeToPair { get; set; }
        ///<summary>
        ///Time to pick up the package in minutes
        ///</summary>
        public double TimeToPickup { get; set; }
        ///<summary>
        ///Time to Deliver in minutes
        ///</summary>
        public double TimeToDeliver { get; set; }
    }
}
