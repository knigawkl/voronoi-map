using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.DataContainers
{
    class CustomObjectType
    {
        public string Name { get; }

        public Dictionary<string, string> Variables { set; get; }

        public CustomObjectType (string name)
        {
            Name = name;
        }

        public void AddVariable(string variableType, string variableName)
        {
            string [] types = {"string", "int", "double", "float", "bool", "long" };
            bool isTypeRecognised = false;
            foreach(string type in types) {
                if(type == variableType)
                {
                    isTypeRecognised = true;
                    break;
                }
            }
            if(!isTypeRecognised)
            {
                throw new Exception("Type is not recognised");
            }
            Variables.Add(variableName, variableType);
        }
    }
}
