using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public static class DalFactory
    {
        public static IDAL GetDal(string param)
        {
            return param switch
            {
                "DalObject" => new DalObject.DalObject(),
                "DalXml" => new DalXml(),
                _ => throw new ArgumentException($"{param} is not a recognized class or interface")
            };
        }
    }
}
