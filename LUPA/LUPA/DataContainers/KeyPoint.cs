using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA
{
    public class KeyPoint : Point
    {
        public string Name { set; get; }
        public KeyPoint(double x, double y, string name) : base(x, y)
        {
            Name = name;
        }
    }
}
