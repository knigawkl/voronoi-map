using System;
using System.Linq;
using System.Windows.Media;

namespace LUPA
{
    public class KeyPoint : Point
    {
        public string Name { set; get; }
        public int Id { set; get; } 
        public KeyPoint(double x, double y, string name) : base(x, y)
        {
            Name = name;
        }
    }
}
