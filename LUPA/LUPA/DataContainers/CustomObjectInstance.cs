using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.DataContainers
{
    class CustomObjectInstance
    {
        public CustomObjectType ObjectType { set; get; }
        public readonly object[] objectProperties;

        public CustomObjectInstance (CustomObjectType objectType, string [] args)
        {
            if(args.Length != objectType.Variables.Count)
            {
                throw new Exception("Wrong number of args in object instance");
            }
            for (int i = 0; i < args.Length; i++)
            {
                //parse args
            }
        }
    }
}
