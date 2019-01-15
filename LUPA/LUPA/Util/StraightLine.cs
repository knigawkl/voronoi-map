using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.Util
{
    public class StraightLine
    {
        public double A { set; get; }
        public double B { set; get; }
        public double C { set; get; }

        public StraightLine(double a, double b, double c)
        {
            A = a;
            B = b;
            C = c;
        }

        public override bool Equals(object obj)
        {
            StraightLine adLine = (StraightLine)obj;
            return Math.Abs(adLine.A - A) < 0.01 && Math.Abs(adLine.B - B) < 0.01 && Math.Abs(adLine.C - C) < 0.01;
        }

        public override int GetHashCode()
        {
            var hashCode = 793064651;
            hashCode = hashCode * -1521134295 + A.GetHashCode();
            hashCode = hashCode * -1521134295 + B.GetHashCode();
            hashCode = hashCode * -1521134295 + C.GetHashCode();
            return hashCode;
        }
    }
}
