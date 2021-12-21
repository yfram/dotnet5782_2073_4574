using System;

namespace DalApi
{
    public static class DalFactory
    {
        public static IDAL GetDal(string param)
        {
            return param switch
            {
                "DalObject" => new DalObject.DalObject(),
                "DalXml" => throw new NotImplementedException(),//new DalXml(),
                _ => throw new ArgumentException($"{param} is not a recognized class or interface")
            };
        }
    }
}
