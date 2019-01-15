using LUPA.Util;
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

        public bool IsIntersecting(LineSegment secondLine)
        {          
            double a, b, c;
            if (secondLine.StartPoint.X != secondLine.EndPoint.X)
            {
                a = (secondLine.StartPoint.Y - secondLine.EndPoint.Y) / (secondLine.StartPoint.X - secondLine.EndPoint.X);
                c = secondLine.StartPoint.Y - a * secondLine.StartPoint.X;
                b = -1;
            }
            else
            {
                a = 1;
                c = -secondLine.StartPoint.X;
                b = 0;
            }
            StraightLine secondStraightLine = new StraightLine(a, b, c);
            if (StartPoint.X != EndPoint.X)
            {
                a = (StartPoint.Y - EndPoint.Y) / (StartPoint.X - EndPoint.X);
                c = StartPoint.Y - a * StartPoint.X;
                b = -1;
            }
            else
            {
                a = 1;
                c = -StartPoint.X;
                b = 0;
            }
            StraightLine currentStraightLine = new StraightLine(a, b, c);
            if(Mathematics.TryGetIntersection(currentStraightLine, secondStraightLine, out Point intersectionPoint))
            {
                if(StartPoint.X - EndPoint.X != 0)
                {
                    if (intersectionPoint.X < Math.Max(StartPoint.X, EndPoint.X) && intersectionPoint.X > Math.Min(StartPoint.X, EndPoint.X)
                        && intersectionPoint.Y < Math.Max(secondLine.StartPoint.Y, secondLine.EndPoint.Y) && intersectionPoint.Y > Math.Min(secondLine.StartPoint.Y, secondLine.EndPoint.Y))
                        return true;
                }
                else
                {
                    if (intersectionPoint.Y < Math.Max(StartPoint.Y, EndPoint.Y) && intersectionPoint.Y > Math.Min(StartPoint.Y, EndPoint.Y)
                        && intersectionPoint.Y < Math.Max(secondLine.StartPoint.X, secondLine.EndPoint.X) && intersectionPoint.X > Math.Min(secondLine.StartPoint.X, secondLine.EndPoint.X))
                        return true;
                }
                
            }
            return false;
        }
    }
}
