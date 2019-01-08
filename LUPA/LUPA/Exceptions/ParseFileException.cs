using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.Exceptions
{
    public class ParseFileException : Exception
    {
        public ParseFileException(string message) : base(message) { }
    }
}
