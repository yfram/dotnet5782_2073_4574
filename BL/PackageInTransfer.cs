﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class PackageInTransfer
    {
        public int Id { get; set; }
        public bool OnTheWay { get; set; }
        public WeightGroup Weight { get; set; }
        public PriorityGroup Priority { get; set; }
        public CustomerForPackage Sender { get; set; }
        public CustomerForPackage Reciver { get; set; }
        public Location PickupLocation { get; set; }
        public Location DropOffLocation { get; set; }
    }
}
