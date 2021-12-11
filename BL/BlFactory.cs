using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public class BlFactory
    {
        internal IBL Instance = null;

        public IBL GetBl()
        {
            return Instance;
        }

        public BlFactory()
        {
            if (Instance == null)
                Instance = new BL();
        }
    }
}
