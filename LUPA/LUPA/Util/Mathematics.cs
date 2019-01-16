using System;
using System.Collections.Generic;
using System.Linq;

namespace LUPA.Util
{
    public static class Mathematics
    {
        public static double CalculateDistBetweenPoints(System.Windows.Point srcPt, System.Windows.Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }

        public static double CalculateDistBetweenPoints(Point srcPt, Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }

        public static double CalculateDistBetweenPoints(System.Windows.Point srcPt, Point endPt)
        {
            return Math.Sqrt(Math.Pow((endPt.X - srcPt.X), 2) + Math.Pow((endPt.Y - srcPt.Y), 2));
        }

        public static bool TryGetIntersection(StraightLine firstLine, LineSegment contourLine, out Point intersectionPoint)
        {
            double a, b, c;
            if (contourLine.StartPoint.X != contourLine.EndPoint.X)
            {
                a = (contourLine.StartPoint.Y - contourLine.EndPoint.Y) / (contourLine.StartPoint.X - contourLine.EndPoint.X);
                c = contourLine.StartPoint.Y - a * contourLine.StartPoint.X;
                b = -1;
            }
            else
            {
                a = 1;
                c = -contourLine.StartPoint.X;
                b = 0;
            }
            StraightLine secondLine = new StraightLine(a, b, c);
            if (!TryGetIntersection(firstLine, secondLine, out intersectionPoint))
            {
                return false;
            }

            if (intersectionPoint.X <= Math.Max(contourLine.StartPoint.X, contourLine.EndPoint.X) && intersectionPoint.X >= Math.Min(contourLine.StartPoint.X, contourLine.EndPoint.X))
            {
                return true;
            }
            return false;
        }

        public static bool TryGetIntersection(StraightLine firstLine, StraightLine secondLine, out Point intersectionPoint)
        {
            if (firstLine.A == secondLine.A)
            {
                intersectionPoint = new Point(0, 0);
                return false;
            }
            else if (firstLine.B == 0)
            {
                double x = -firstLine.C;
                double y = (-x * secondLine.A) + secondLine.C;
                intersectionPoint = new Point(x, y);
                return true;
            }
            else if (secondLine.B == 0)
            {
                double x = -secondLine.C;
                double y = (-x * firstLine.A) + firstLine.C;
                intersectionPoint = new Point(x, y);
                return true;
            }
            else
            {
                double x = (secondLine.C - firstLine.C) / (firstLine.A - secondLine.A);
                double y = firstLine.A * x + firstLine.C;
                intersectionPoint = new Point(x, y);
                return true;
            }

        }

        public static bool IsPointInPolygon(List<Point> polygon, Point p)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Count; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            bool inside = false;
            for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
