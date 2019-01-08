using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA
{
    public class LineSegment
    {
        public Point StartPoint { set; get; }
        public Point EndPoint { set; get; }

        public LineSegment (Point startPoint, Point endPoint)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
        }
    }
}
