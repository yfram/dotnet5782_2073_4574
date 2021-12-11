using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public static class BlFactory
    {
        internal static IBL Instance = null;

        public static IBL GetBl()
        {
            if (Instance == null)
                Instance = new BL();
            return Instance;
        }

    }
}
