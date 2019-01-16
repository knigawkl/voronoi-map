using System;
using System.Linq;
using System.Windows.Media;

namespace LUPA
{
    public class KeyPoint : Point
    {
        public string Name { set; get; }
        public SolidColorBrush BrushColor { set; get; } 
        public KeyPoint(double x, double y, string name) : base(x, y)
        {
            Name = name;
            //BrushColor = Brushes.Black;
            //SolidColorBrush dupa = new SolidColorBrush(Color.FromRgb(124, 12, 12));
            //BrushColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFF7F50")); 
        }
    }
}
