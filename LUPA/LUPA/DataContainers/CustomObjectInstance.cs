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
            if(args.Length != objectType.VariableNames.Capacity)
            {
                throw new Exception("Wrong number of args in object instance");
            }
            objectProperties = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                switch (objectType.VariableTypes[i])
                {
                    case "int":
                        if (!int.TryParse(args[i], out int argI))
                        {
                            throw new Exception(i + ". argument is not integer type");
                        }
                        objectProperties[i] = argI;
                        break;
                    case "double":
                        if (!double.TryParse(args[i], out double argD))
                        {
                            throw new Exception(i + ". argument is not double type");
                        }
                        objectProperties[i] = argD;
                        break;
                    case "float":
                        if (!float.TryParse(args[i], out float argF))
                        {
                            throw new Exception(i + ". argument is not float type");
                        }
                        objectProperties[i] = argF;
                        break;
                    case "string":
                        objectProperties[i] = args[i];
                        break;
                    case "bool":
                        if (!bool.TryParse(args[i], out bool argB))
                        {
                            throw new Exception(i + ". argument is not boolean type");
                        }
                        objectProperties[i] = argB;
                        break;
                    case "long":
                        if (!long.TryParse(args[i], out long argL))
                        {
                            throw new Exception(i + ". argument is not integer type");
                        }
                        objectProperties[i] = argL;
                        break;
                }


            }
        }
    }
}
