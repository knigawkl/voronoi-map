using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.Utils
{
    class Utils
    {
        private double CalculateDistBetweenPoints(System.Windows.Point srcPt, System.Windows.Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }

        private double CalculateDistBetweenPoints(Point srcPt, Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }

        private double CalculateDistBetweenPoints(System.Windows.Point srcPt, Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }
    }
}
