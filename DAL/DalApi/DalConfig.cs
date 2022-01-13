// File {filename} created by Yoni Fram and Gil Kovshi
// All rights reserved

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DalApi
{
    internal class DalConfig
    {
        internal static string DalName;
        internal static Dictionary<string, string> DalPackages;
        internal static Dictionary<string, string> DalClass;
        internal static Dictionary<string, string> DalNamspace;
        internal static Dictionary<string, string> DalInstance;

        static DalConfig()
        {
            Refresh();
        }

        public static void Refresh()
        {
            XElement dalConfig = XElement.Load(@"xml\dal-config.xml");
            DalName = dalConfig.Element("dal").Value;
            DalPackages = (from pkg in dalConfig.Element("dal-packages").Elements()
                           select pkg
                          ).ToDictionary(p => "" + p.Name, p => p.Value);
            DalClass = (from pkg in dalConfig.Element("dal-packages").Elements()
                        select pkg
                          ).ToDictionary(p => "" + p.Name, p => p.Attribute("class").Value);
            DalNamspace = (from pkg in dalConfig.Element("dal-packages").Elements()
                           select pkg
                          ).ToDictionary(p => "" + p.Name, p => p.Attribute("namespace").Value);

            DalInstance = (from pkg in dalConfig.Element("dal-packages").Elements()
                           select pkg
                          ).ToDictionary(p => "" + p.Name, p => p.Attribute("instance").Value);
        }
    }
    public class DalConfigException : Exception
    {
        public DalConfigException(string msg) : base(msg) { }
        public DalConfigException(string msg, Exception ex) : base(msg, ex) { }
    }
}