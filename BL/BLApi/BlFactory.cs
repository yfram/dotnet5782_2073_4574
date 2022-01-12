// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

namespace BlApi
{

    /// <summary>
    /// A factory for BL
    /// </summary>
    public static class BlFactory
    {
        internal static IBL Instance = null;

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The BL instance</returns>
        public static IBL GetBl()
        {
            if (Instance == null)
            {
                Instance = new BL();
            }

            return Instance;
        }

    }
}
