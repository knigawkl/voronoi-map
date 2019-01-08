using LUPA.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.DataContainers
{
    public class CustomObjectType
    {
        public string Name { get; }

        public List<string> VariableNames { set; get; } 
        public List<string> VariableTypes { set; get; } 

        public CustomObjectType (string name)
        {
            Name = name;
            VariableNames = new List<string>();
            VariableTypes = new List<string>();
        }

        public void AddVariable(string variableName, string variableType)
        {
            string [] types = {"string", "String", "int", "double", "float", "bool", "long" };
            bool isTypeRecognised = false;
            foreach(string type in types) {
                if(type.Equals(variableType))
                {
                    isTypeRecognised = true;
                    break;
                }
            }
            if(!isTypeRecognised)
            {
                throw new ObjectTypeDeclarationException("Type is not recognised");
            }
            VariableNames.Add(variableName);
            VariableTypes.Add(variableType);
        }
    }
}
