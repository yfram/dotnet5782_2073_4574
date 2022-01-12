using System;
using System.Reflection;

namespace DalApi
{
    public class DalFactory
    {
        public static IDAL GetDal(bool force = false)
        {

            string dalType = DalConfig.DalName;
            string dalPkg = DalConfig.DalPackages[dalType];
            string dalClass = DalConfig.DalClass[dalType];
            string dalNamspace = DalConfig.DalNamspace[dalType];
            string DalInstance = DalConfig.DalInstance[dalType];

            if (force)
            {
                dalType = "list";
                dalPkg = "DalObject";
            }

            if (dalPkg == null)
            {
                throw new DalConfigException($"Package {dalType} is not found in the package list in dal-config.xml");
            }

            try { Assembly.Load(dalPkg); }
            catch (Exception) { throw new DalConfigException("Failed to load dal-config.xml"); }

            Type type = Type.GetType($"{dalNamspace}.{dalClass}, {dalPkg}");
            if (type == null)
            {
                throw new DalConfigException($"Class {dalClass} was not found in the {dalPkg}.dll");
            }

            IDAL dal = (IDAL)type.GetProperty(DalInstance,
                      BindingFlags.Public | BindingFlags.Static).GetValue(null);
            if (dal == null)
            {
                throw new DalConfigException($"Class {dalPkg} is not a singleton or a wrong propertry name for Instance was given");
            }

            return dal;
        }
    }
}