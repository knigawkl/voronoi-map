using LUPA.DataContainers;
using LUPA.Util;
using System;
using System.Collections.Generic;

namespace LUPA
{
    public class AreaDivider
    {
       
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
                firstAngle = firstPointRef.Y > 0 ? Math.PI/2 : 3 * Math.PI / 2;
            }

            if (thirdPointRef.X != 0)
            {
                double thirdTan = thirdPointRef.Y / thirdPointRef.X;
                thirdAngle = Math.Atan(thirdTan);
            }
            else
            {
                thirdAngle = thirdPointRef.Y > 0 ? Math.PI / 2 : 3 * Math.PI / 2;
            }

            
            if (firstPointRef.X < 0)
            {
                firstAngle += Math.PI;
            }
            if (firstAngle < 0)
            {
                firstAngle += 2*Math.PI;
            }

            if (thirdPointRef.X < 0)
            {
                thirdAngle += Math.PI;
            }
            if (thirdAngle < 0)
            {
                thirdAngle += 2*Math.PI;
            }
            
            double finalAngle = thirdAngle + (2 * Math.PI - firstAngle);
            if (finalAngle > 2 * Math.PI)
            {
                finalAngle -= 2 * Math.PI;
            }
            return finalAngle > Math.PI;
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
                List<StraightLine> bisectors = new List<StraightLine>();
                foreach (KeyPoint anotherKeyPoint in map.KeyPoints)
                {
                    if (!keyPoint.Equals(anotherKeyPoint))
                    {
                        bisectors.Add(CreateBisector(keyPoint, anotherKeyPoint));
                    }
                }

                StraightLine closestBisector = bisectors[0];
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
                List<StraightLine> bisectorsIntersecting = new List<StraightLine>();
                foreach (StraightLine bisector in bisectors)
                {
                    if (!closestBisector.Equals(bisector))
                    {
                        if (Util.Mathematics.TryGetIntersection(closestBisector, bisector, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(false);
                            bisectorsIntersecting.Add(bisector);
                        }
                    }
                }
                foreach (LineSegment contourLine in contourLines)
                {
                    if (Util.Mathematics.TryGetIntersection(closestBisector, contourLine, out Point intersectionPoint))
                    {
                        intersectionPoints.Add(intersectionPoint);
                        isPointContourPoint.Add(true);
                        bisectorsIntersecting.Add(null);
                    }
                }
                Point closestIntersectionPoint = intersectionPoints[0];
                double closestIntersectionPointDistance = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[0], closestPoint);
                int closestIntersectionPointIterator = 0;
                for (int i = 1; i < intersectionPoints.Count; i++)
                {
                    double dst = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[i], closestPoint);
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
                            double dst = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[i], closestPoint);
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
                            double dst = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[i], closestPoint);
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
                Point originalLeftPoint = leftPoint;
                while (!isLeftFinished)
                {
                    StraightLine currentBisector = bisectorsIntersecting[closestIntersectionPointIteratorLeft];
                    intersectionPoints.Clear();
                    isPointContourPoint.Clear();
                    bisectorsIntersecting.Clear();
                    foreach (StraightLine bisector in bisectors)
                    {
                        if (!currentBisector.Equals(bisector))
                        {
                            if (Util.Mathematics.TryGetIntersection(currentBisector, bisector, out Point intersectionPoint))
                            {
                                intersectionPoints.Add(intersectionPoint);
                                isPointContourPoint.Add(false);
                                bisectorsIntersecting.Add(bisector);
                            }
                        }
                    }
                    foreach (LineSegment contourLine in contourLines)
                    {
                        if (Util.Mathematics.TryGetIntersection(currentBisector, contourLine, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(true);
                            bisectorsIntersecting.Add(null);
                        }
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(leftPoint))
                        {
                            double dst = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[i], leftPoint);
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
                    if (leftPoint.Equals(rightPoint))
                    {
                        isLeftFinished = true;
                        isRightFinished = true;
                    }
                }
                while (!isRightFinished)
                {
                    StraightLine currentBisector = bisectorsIntersecting[closestIntersectionPointIteratorRight];
                    intersectionPoints.Clear();
                    isPointContourPoint.Clear();
                    bisectorsIntersecting.Clear();
                    foreach (StraightLine bisector in bisectors)
                    {
                        if (!currentBisector.Equals(bisector))
                        {
                            if (Util.Mathematics.TryGetIntersection(currentBisector, bisector, out Point intersectionPoint))
                            {
                                intersectionPoints.Add(intersectionPoint);
                                isPointContourPoint.Add(false);
                                bisectorsIntersecting.Add(bisector);
                            }
                        }
                    }
                    foreach (LineSegment contourLine in contourLines)
                    {
                        if (Util.Mathematics.TryGetIntersection(currentBisector, contourLine, out Point intersectionPoint))
                        {
                            intersectionPoints.Add(intersectionPoint);
                            isPointContourPoint.Add(true);
                            bisectorsIntersecting.Add(null);
                        }
                    }
                    closestIntersectionPointDistance = double.PositiveInfinity;
                    closestIntersectionPointIterator = -1;
                    for (int i = 0; i < intersectionPoints.Count; i++)
                    {
                        if (!intersectionPoints[i].Equals(rightPoint))
                        { 
                            double dst = Util.Mathematics.CalculateDistBetweenPoints(intersectionPoints[i], rightPoint);
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

        public static double Distance(Point point, StraightLine line, out Point closestPoint)
        {
            if (line.B != 0 && line.A != 0)
            {
                StraightLine perpendicular = new StraightLine(-(1 / line.A), -1, point.Y + (point.X / line.A));
                Util.Mathematics.TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(line.A * point.X - point.Y + line.C) / Math.Sqrt(Math.Pow(line.A, 2) + 1);
            }
            else if (line.B != 0 && line.A == 0)
            {
                StraightLine perpendicular = new StraightLine(1, 0, -point.X);
                Util.Mathematics.TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(point.Y - line.C);
            }
            else
            {
                StraightLine perpendicular = new StraightLine(0, -1, point.Y);
                Util.Mathematics.TryGetIntersection(line, perpendicular, out closestPoint);
                return Math.Abs(point.X - line.C);
            }
        }

        public static StraightLine CreateBisector(KeyPoint firstKeyPoint, KeyPoint secondKeyPoint)
        {
            double a, b, c;
            Point midPoint = new Point((firstKeyPoint.X + secondKeyPoint.X) / 2, (firstKeyPoint.Y + secondKeyPoint.Y) / 2);
            if (firstKeyPoint.X != secondKeyPoint.X)
            {
                a = (firstKeyPoint.Y - secondKeyPoint.Y) / (firstKeyPoint.X - secondKeyPoint.X);
                c = firstKeyPoint.Y - a * firstKeyPoint.X;
                b = -1;
                StraightLine line = new StraightLine(a, b, c);
                return new StraightLine(-(1 / line.A), -1, midPoint.Y + (midPoint.X / line.A));
            }
            else
            {
                a = 1;
                c = -firstKeyPoint.X;
                b = 0;
                StraightLine line = new StraightLine(a, b, c);
                return new StraightLine(0, -1, midPoint.Y);
            }
            
            

        }
    }
}
