namespace BlApi
{
    public static class BlFactory
    {
        internal static IBL Instance = null;

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
