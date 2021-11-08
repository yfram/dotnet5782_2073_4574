using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public enum WeightGroup
    {
        Light, Mid, Heavy
    }
    public enum PriorityGroup
    {
        Normal, Fast, Urgent
    }
    public enum PackageStatus
    {
        Initialized, Paired, PickedUp, Accepted
    }
    public enum DroneState
    {
        Empty, Maitenance, Bussy
    }
}
