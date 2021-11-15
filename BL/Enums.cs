using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public enum WeightGroup
    {
        Light=1, Mid, Heavy
    }
    public enum PriorityGroup
    {
        Normal=1, Fast, Urgent
    }
    public enum PackageStatus
    {
        Initialized=1, Paired, PickedUp, Accepted
    }
    public enum DroneState
    {
        Empty=1, Maitenance, Bussy
    }
}
