using System;
using System.Reflection;

namespace DalApi
{
    public class DalFactory
    {
        public static IDAL GetDal()
        {
            string dalType = DalConfig.DalName;
            string dalPkg = DalConfig.DalPackages[dalType];
            if (dalPkg == null) throw new DalConfigException($"Package {dalType} is not found in packages list in dal-config.xml");

            try { Assembly.Load(dalPkg); }
            catch (Exception) { throw new DalConfigException("Failed to load the dal-config.xml file"); }

            Type type = Type.GetType($"Dal.{dalPkg}, {dalPkg}");
            if (type == null) throw new DalConfigException($"Class {dalPkg} was not found in the {dalPkg}.dll");

            IDAL dal = (IDAL)type.GetProperty("Instance",
                      BindingFlags.Public | BindingFlags.Static).GetValue(null);
            if (dal == null) throw new DalConfigException($"Class {dalPkg} is not a singleton or wrong propertry name for Instance");

            return dal;
        }
    }
}