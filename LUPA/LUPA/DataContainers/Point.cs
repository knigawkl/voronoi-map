using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LUPA.AreaDivider;

namespace LUPA
{
    public class Point
    {
        public double X { set; get; }
        public double Y { set; get; }

        public Point (double x, double y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            Point point = (Point)obj;
            //return point.X == X && point.Y == Y;
            return CalculateDistBetweenPoints(this, point) < 0.01;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }
    }
}
