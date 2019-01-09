using LUPA.DataContainers;
using System;
using System.Collections.Generic;

namespace LUPA
{
    public class AreaDivider
    {
        private class ADLine
        {
            public double A { set; get; }
            public double B { set; get; }

            public ADLine(double a, double b)
            {
                A = a;
                B = b;
            }
        }

        public static bool IsPointOnRight(Point firstPoint, Point secondPoint, Point thirdPoint)
        {
            Point firstPointRef = new Point(firstPoint.X - secondPoint.X, firstPoint.Y - secondPoint.Y);
            Point secondPointRef = new Point(0, 0);
            Point thirdPointRef = new Point(thirdPoint.X - secondPoint.X, thirdPoint.Y - secondPoint.Y);
            double firstTan = firstPointRef.Y / firstPointRef.X;
            double thirdTan = thirdPointRef.Y / thirdPointRef.X;
            double firstAngle = Math.Atan(firstTan);
            if (firstAngle < 0)
            {
                firstAngle = Math.PI - firstAngle;
            }
            if (firstPointRef.X < 0)
            {
                firstAngle += Math.PI;
            }
            double thirdAngle = Math.Atan(thirdTan);
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
            return finalAngle > Math.PI ? false : true;
        }

        public static void DivideIntoAreas(Map map)
        {
            List<LineSegment> contourLines = new List<LineSegment>();
            //todo contourLines
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

        private static double Distance(Point firstPoint, Point secondPoint)
        {
            throw new NotImplementedException();
        }

        private static bool TryGetIntersection(ADLine bisector, LineSegment contourLine, out Point intersectionPoint)
        {
            throw new NotImplementedException();
        }

        private static bool TryGetIntersection(ADLine firstBisector, ADLine secondBisector, out Point intersectionPoint)
        {
            throw new NotImplementedException();
        }

        private static double Distance(Point point, ADLine bisector, out Point closestPoint)
        {
            throw new NotImplementedException();
        }

        private static ADLine CreateBisector(KeyPoint keyPoint, KeyPoint anotherKeyPoint)
        {
            throw new NotImplementedException();
        }
    }
}
