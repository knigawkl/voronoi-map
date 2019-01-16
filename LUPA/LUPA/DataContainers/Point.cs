using LUPA.Util;

namespace LUPA
{
    public class Point
    {
        public int X { set; get; }
        public int Y { set; get; }

        public Point (double x, double y)
        {
            X = (int)x;
            Y = (int)y;
        }

        public override bool Equals(object obj)
        {
            Point point = (Point)obj;
            return Mathematics.CalculateDistBetweenPoints(this, point) < 0.01;
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
