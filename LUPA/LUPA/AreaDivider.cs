using LUPA.DataContainers;
using System;
using System.Collections.Generic;

namespace LUPA
{
    public class AreaDivider
    {
        public class ADLine
        {
            public double A { set; get; }
            public double B { set; get; }
            public double C { set; get; }

            public ADLine(double a, double b, double c)
            {
                A = a;
                B = b;
                C = c;
            }

            public override bool Equals(object obj)
            {
                ADLine adLine = (ADLine)obj;
                return adLine.A == A && adLine.B == B && adLine.C == C;
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

        public static bool IsPointOnRight(Point firstPoint, Point secondPoint, Point thirdPoint)
        {
            Point firstPointRef = new Point(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
            Point secondPointRef = new Point(0, 0);
            Point thirdPointRef = new Point(thirdPoint.X - secondPoint.X, thirdPoint.Y - secondPoint.Y);
            double firstAngle, thirdAngle;
            if(firstPointRef.X != 0)
            {
                double firstTan = firstPointRef.Y / firstPointRef.X;
                firstAngle = Math.Atan(firstTan);
            }
            else
            {
                firstAngle = firstPointRef.X > 0 ? 0 : Math.PI;
            }

            if (thirdPointRef.X != 0)
            {
                double thirdTan = thirdPointRef.Y / thirdPointRef.X;
                thirdAngle = Math.Atan(thirdTan);
            }
            else
            {
                thirdAngle = thirdPointRef.X > 0 ? 0 : Math.PI;
            }

            if (firstAngle < 0)
            {
                firstAngle = Math.PI - firstAngle;
            }
            if (firstPointRef.X < 0)
            {
                firstAngle += Math.PI;
            }
            
            if (firstAngle < 0)
            {
                thirdAngle = Math.PI / 2 - thirdAngle;
            }
            if (firstPointRef.X < 0)
            {
                thirdAngle += Math.PI;
            }
            double finalAngle = thirdAngle + (2 * Math.PI - firstAngle);
            if (finalAngle > 2 * Math.PI)
            {
                finalAngle -= 2 * Math.PI;
            }
            return finalAngle > Math.PI ? true : false;
        }

        public static void DivideIntoAreas(Map map)
        {
            List<LineSegment> contourLines = new List<LineSegment>();
            for(int i = 0; i < map.ContourPoints.Count - 1; i++)
            {
                contourLines.Add(new LineSegment(map.ContourPoints[i], map.ContourPoints[i + 1]));
            }
            contourLines.Add(new LineSegment(map.ContourPoints[map.ContourPoints.Count - 1], map.ContourPoints[0]));
            foreach (KeyPoint keyPoint in map.KeyPoints)
            {
                List<ADLine> bisectors = new List<ADLine>();
                foreach (KeyPoint anotherKeyPoint in map.KeyPoints)
                {
                    if (!keyPoint.Equals(anotherKeyPoint))
                    {
                        bisectors.Add(CreateBisector(keyPoint, anotherKeyPoint));
                    }
                }

                ADLine closestBisector = bisectors[0];
                double closestBisectorDistance = Distance(keyPoint, closestBisector, out Point closestPoint);
                for (int i = 1; i < bisectors.Count; i++)
                {
                    double bisectorDistance = Distance(keyPoint, bisectors[i], out Point possibleClosestPoint);
                    if (bisectorDistance < closestBisectorDistance)
                    {
                        closestBisectorDistance = bisectorDistance;
                        closestBisector = bisectors[i];
                        closestPoint = possibleClosestPoint;
                    }
                }
                List<Point> intersectionPoints = new List<Point>();
                List<bool> isPointContourPoint = new List<bool>();
                foreach (ADLine bisector in bisectors)
                {
                    if (!closestBisector.Equals(bisector))
                    {
                        if (TryGetIntersection(closestBisector, bisector, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(false);
                        }
                    }
                }
                foreach (LineSegment contourLine in contourLines)
                {
                    if (TryGetIntersection(closestBisector, contourLine, out Point intersectionPoint))
                    {
                        intersectionPoints.Add(intersectionPoint);
                        isPointContourPoint.Add(true);
                    }
                }
                Point closestIntersectionPoint = intersectionPoints[0];
                double closestIntersectionPointDistance = Distance(intersectionPoints[0], closestPoint);
                int closestIntersectionPointIterator = 0;
                for (int i = 1; i < intersectionPoints.Count; i++)
                {
                    double dst = Distance(intersectionPoints[i], closestPoint);
                    if (dst < closestIntersectionPointDistance)
                    {
                        closestIntersectionPoint = intersectionPoints[i];
                        closestIntersectionPointDistance = dst;
                        closestIntersectionPointIterator = i;
                    }
                }
                bool isRightFinished = false, isLeftFinished = false;
                Point leftPoint, rightPoint, prevLeftPoint, prevRightPoint;
                int closestIntersectionPointIteratorLeft, closestIntersectionPointIteratorRight;
                if (IsPointOnRight(keyPoint, closestPoint, closestIntersectionPoint))
                {
                    rightPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorRight = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isRightFinished = true;
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(rightPoint))
                        {
                            double dst = Distance(intersectionPoints[i], closestPoint);
                            if (dst < closestIntersectionPointDistance && !IsPointOnRight(keyPoint, closestPoint, intersectionPoints[i]))
                            {
                                closestIntersectionPoint = intersectionPoints[i];
                                closestIntersectionPointDistance = dst;
                                closestIntersectionPointIterator = i;
                            }
                        }
                    }
                    leftPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorLeft = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isLeftFinished = true;
                    }
                }
                else
                {
                    leftPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorLeft = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isLeftFinished = true;
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(leftPoint))
                        {
                            double dst = Distance(intersectionPoints[i], closestPoint);
                            if (dst < closestIntersectionPointDistance && IsPointOnRight(keyPoint, closestPoint, intersectionPoints[i]))
                            {
                                closestIntersectionPoint = intersectionPoints[i];
                                closestIntersectionPointDistance = dst;
                                closestIntersectionPointIterator = i;
                            }
                        }
                    }
                    rightPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorRight = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isRightFinished = true;
                    }
                }
                //not working with three in a row
                prevLeftPoint = prevRightPoint = closestPoint;
                map.AreaLineSegments.Add(new LineSegment(leftPoint, rightPoint));
                while (!isLeftFinished)
                {
                    ADLine currentBisector = bisectors[closestIntersectionPointIteratorLeft];
                    intersectionPoints.Clear();
                    isPointContourPoint.Clear();
                    foreach (ADLine bisector in bisectors)
                    {
                        if (!currentBisector.Equals(bisector))
                        {
                            if (TryGetIntersection(currentBisector, bisector, out Point intersectionPoint))
                            {
                                intersectionPoints.Add(intersectionPoint);
                                isPointContourPoint.Add(false);
                            }
                        }
                    }
                    foreach (LineSegment contourLine in contourLines)
                    {
                        if (TryGetIntersection(currentBisector, contourLine, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(true);
                        }
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(leftPoint))
                        {
                            double dst = Distance(intersectionPoints[i], leftPoint);
                            if (dst < closestIntersectionPointDistance && !IsPointOnRight(prevLeftPoint, leftPoint, intersectionPoints[i]))
                            {
                                closestIntersectionPoint = intersectionPoints[i];
                                closestIntersectionPointDistance = dst;
                                closestIntersectionPointIterator = i;
                            }
                        }
                    }
                    prevLeftPoint = leftPoint;
                    leftPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorLeft = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isLeftFinished = true;
                    }
                    map.AreaLineSegments.Add(new LineSegment(prevLeftPoint, leftPoint));
                }
                while (!isRightFinished)
                {
                    ADLine currentBisector = bisectors[closestIntersectionPointIteratorRight];
                    intersectionPoints.Clear();
                    isPointContourPoint.Clear();
                    foreach (ADLine bisector in bisectors)
                    {
                        if (!currentBisector.Equals(bisector))
                        {
                            if (TryGetIntersection(currentBisector, bisector, out Point intersectionPoint))
                            {
                                intersectionPoints.Add(intersectionPoint);
                                isPointContourPoint.Add(false);
                            }
                        }
                    }
                    foreach (LineSegment contourLine in contourLines)
                    {
                        if (TryGetIntersection(currentBisector, contourLine, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(true);
                        }
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(rightPoint))
                        {
                            double dst = Distance(intersectionPoints[i], rightPoint);
                            if (dst < closestIntersectionPointDistance && IsPointOnRight(prevRightPoint, rightPoint, intersectionPoints[i]))
                            {
                                closestIntersectionPoint = intersectionPoints[i];
                                closestIntersectionPointDistance = dst;
                                closestIntersectionPointIterator = i;
                            }
                        }
                    }
                    prevRightPoint = rightPoint;
                    rightPoint = closestIntersectionPoint;
                    closestIntersectionPointIteratorRight = closestIntersectionPointIterator;
                    if (isPointContourPoint[closestIntersectionPointIterator])
                    {
                        isRightFinished = true;
                    }
                    map.AreaLineSegments.Add(new LineSegment(prevRightPoint, rightPoint));
                }



            }
        }

        public static double Distance(Point firstPoint, Point secondPoint)
        {
            return Math.Sqrt(Math.Pow((secondPoint.X - firstPoint.X), 2) + Math.Pow((secondPoint.Y - firstPoint.Y), 2));
        }

        public static bool TryGetIntersection(ADLine firstLine, LineSegment contourLine, out Point intersectionPoint)
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
            ADLine secondLine = new ADLine(a, b, c);
            if (!TryGetIntersection(firstLine, secondLine, out intersectionPoint))
            {
                return false;
            }
            
            if(intersectionPoint.X <= Math.Max(contourLine.StartPoint.X, contourLine.EndPoint.X) && intersectionPoint.X >= Math.Min(contourLine.StartPoint.X, contourLine.EndPoint.X))
            {
                return true;
            }
            return false;
        }

        public static bool TryGetIntersection(ADLine firstLine, ADLine secondLine, out Point intersectionPoint)
        {
            if(firstLine.A == secondLine.A)
            {
                intersectionPoint = new Point(0, 0);
                return false;
            }
            else if (firstLine.B == 0)
            {
                double x = -firstLine.C;
                double y = (-x * secondLine.A) - secondLine.C;
                intersectionPoint = new Point(x, y);
                return true;
            }
            else if (secondLine.B == 0)
            {
                double x = -secondLine.C;
                double y = (-x * firstLine.A) - firstLine.C;
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

        public static double Distance(Point point, ADLine line, out Point closestPoint)
        {
            if (line.B != 0 && line.A != 0)
            {
                ADLine perpendicular = new ADLine(-(1 / line.A), -1,  point.Y + (point.X / line.A));
                TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(line.A * point.X - point.Y + line.C) / Math.Sqrt(Math.Pow(line.A, 2) + 1);
            }
            else if(line.B != 0 && line.A == 0)
            {
                ADLine perpendicular = new ADLine(1, 0, -point.X);
                TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(point.Y - line.C);
            }
            else
            {
                ADLine perpendicular = new ADLine(0, -1, point.Y);
                TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(point.X - line.C);
            }
        }

        public static ADLine CreateBisector(KeyPoint firstKeyPoint, KeyPoint secondKeyPoint)
        {
            double a, b, c;
            Point midPoint = new Point((firstKeyPoint.X + secondKeyPoint.X) / 2, (firstKeyPoint.Y + secondKeyPoint.Y) / 2);
            if (firstKeyPoint.X != secondKeyPoint.X)
            {
                a = (firstKeyPoint.Y - secondKeyPoint.Y) / (firstKeyPoint.X - secondKeyPoint.X);
                c = firstKeyPoint.Y - a * firstKeyPoint.X;
                b = -1;
                ADLine line = new ADLine(a, b, c);
                return new ADLine(-(1 / line.A), -1, midPoint.Y + (midPoint.X / line.A));
            }
            else
            {
                a = 1;
                c = -firstKeyPoint.X;
                b = 0;
                ADLine line = new ADLine(a, b, c);
                return new ADLine(0, -1, midPoint.Y);
            }
            
            

        }
    }
}
