using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LUPA
{
    public class KeyPoint : Point
    {
        public string Name { set; get; }
        public SolidColorBrush Color { set; get; } 
        public KeyPoint(double x, double y, string name) : base(x, y)
        {
            Name = name;
            SetColor();
        }

        private void SetColor() {
            SolidColorBrush result = Brushes.Transparent;

            Random rnd = new Random();

            Type brushesType = typeof(Brushes);

            PropertyInfo[] properties = brushesType.GetProperties();

            int random = rnd.Next(properties.Length);
            result = (SolidColorBrush)properties[random].GetValue(null, null);

            Color = result;
        }
    }
}
