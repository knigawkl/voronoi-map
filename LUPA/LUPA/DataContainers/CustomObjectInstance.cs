using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.DataContainers
{
    public class CustomObjectInstance : Point
    {
        public CustomObjectType ObjectType { set; get; }
        public readonly object[] objectProperties;

        public CustomObjectInstance (double x, double y, CustomObjectType objectType, object [] objectProperties) : base(x, y)
        {
            ObjectType = objectType;
            this.objectProperties = objectProperties;
            
        }
    }
}
