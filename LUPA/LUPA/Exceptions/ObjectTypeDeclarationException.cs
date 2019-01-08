using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.Exceptions
{
    public class ObjectTypeDeclarationException : Exception
    {
        public ObjectTypeDeclarationException(string message) : base(message) { }
    }
}
