using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Package
    {


        public Package(int id, CustomerForPackage sender, CustomerForPackage reciver, WeightGroup weight, PriorityGroup priority, DroneForPackage drone, DateTime timeToPackage, DateTime timeToPair, DateTime timeToPickup, DateTime timeToDeliver)
        {
            Id = id;
            Sender = sender;
            Reciver = reciver;
            Weight = weight;
            Priority = priority;
            Drone = drone;
            TimeToPackage = timeToPackage;
            TimeToPair = timeToPair;
            TimeToPickup = timeToPickup;
            TimeToDeliver = timeToDeliver;
        }

        public Package()
        {
        }


        public int Id { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Reciver { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public DroneForPackage Drone { get; set; }
        ///<summary>
        ///Time to package in minutes
        ///</summary>
        public DateTime TimeToPackage { get; set; }
        ///<summary>
        ///Time to pair in minutes
        ///</summary>
        public DateTime TimeToPair { get; set; }
        ///<summary>
        ///Time to pick up the package in minutes
        ///</summary>
        public DateTime TimeToPickup { get; set; }
        ///<summary>
        ///Time to Deliver in minutes
        ///</summary>
        public DateTime TimeToDeliver { get; set; }
    }
}
