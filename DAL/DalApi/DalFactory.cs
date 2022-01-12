// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

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

            Type type = Type.GetType($"Dal.{dalPkg}, {dalPkg}");
            if (type == null)
            {
                throw new DalConfigException($"Class {dalPkg} was not found in the {dalPkg}.dll");
            }

            IDAL dal = (IDAL)type.GetProperty("Instance",
                      BindingFlags.Public | BindingFlags.Static).GetValue(null);
            if (dal == null)
            {
                throw new DalConfigException($"Class {dalPkg} is not a singleton or a wrong property name for Instance was given");
            }

            return dal;
        }
    }
}