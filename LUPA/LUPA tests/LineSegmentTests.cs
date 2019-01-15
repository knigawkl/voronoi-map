using LUPA;
using LUPA.Util;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA_tests
{
    [TestFixture]
    class LineSegmentTests
    {
        [Test]
        public void TestSetAndGet()
        {
            //Arrange
            Point point1 = new Point(1, 1);
            Point point2 = new Point(3, 3);
            Point point3 = new Point(5, 4);
            //Act
            LineSegment lineSegment = new LineSegment(point1, point2);         
            //Assert
            Assert.AreEqual(1, lineSegment.StartPoint.X);
            Assert.AreEqual(1, lineSegment.StartPoint.Y);
            Assert.AreEqual(3, lineSegment.EndPoint.X);
            Assert.AreEqual(3, lineSegment.EndPoint.Y);
            //Act
            lineSegment.EndPoint = point3;
            //Assert
            Assert.AreEqual(5, lineSegment.EndPoint.X);
            Assert.AreEqual(4, lineSegment.EndPoint.Y);
        }

        [Test]
        public void TestIntersectionTrue()
        {
            //Arrange
            LineSegment lineSegment1 = new LineSegment(new Point(1, 1), new Point(2, 4));
            LineSegment lineSegment2 = new LineSegment(new Point(1, 3), new Point(4, 1));
            //Act
            bool result = lineSegment1.IsIntersecting(lineSegment2);
            //Assert
            Assert.True(result);
        }

        [Test]
        public void TestIntersectionFalse()
        {
            //Arrange
            LineSegment lineSegment1 = new LineSegment(new Point(1, 1), new Point(2, 4));
            LineSegment lineSegment2 = new LineSegment(new Point(2, 2), new Point(4, 1));
            //Act
            bool result = lineSegment1.IsIntersecting(lineSegment2);
            //Assert
            Assert.False(result);
        }

        [Test]
        public void TestIntersectionFalseParallel()
        {
            //Arrange
            LineSegment lineSegment1 = new LineSegment(new Point(0, 0), new Point(2, 2));
            LineSegment lineSegment2 = new LineSegment(new Point(0, 2), new Point(2, 4));
            //Act
            bool result = lineSegment1.IsIntersecting(lineSegment2);
            //Assert
            Assert.False(result);
        }

        [Test]
        public void TestIntersectionFalsePerpendicular()
        {
            //Arrange
            LineSegment lineSegment1 = new LineSegment(new Point(1, 1), new Point(4, 1));
            LineSegment lineSegment2 = new LineSegment(new Point(2, 2), new Point(2, 5));
            //Act
            bool result = lineSegment1.IsIntersecting(lineSegment2);
            //Assert
            Assert.False(result);
        }

        public void TestIntersectionTruePerpendicular()
        {
            //Arrange
            LineSegment lineSegment1 = new LineSegment(new Point(0, 0), new Point(2, 2));
            LineSegment lineSegment2 = new LineSegment(new Point(0, 2), new Point(2, 5));
            //Act
            bool result = lineSegment1.IsIntersecting(lineSegment2);
            //Assert
            Assert.True(result);
        }
    }
}
