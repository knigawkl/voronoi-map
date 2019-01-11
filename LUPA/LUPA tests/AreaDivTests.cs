using LUPA;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LUPA.AreaDivider;

namespace LUPA_tests
{
    [TestFixture]
    class AreaDivTests
    {
        [Test]
        public void TestDistanceAEqualToZero()
        {
            //Arrange
            Point point = new Point(1, 1);
            ADLine aDLine = new ADLine(0, -1, 10);
            //Act
            Distance(point, aDLine, out Point result);
            //Assert
            Assert.AreEqual(1, result.X);
        }

        [Test]
        public void TestIsPointOnRight()
        {
            //Arrange
            Point fPoint = new Point(5, 5);
            Point sPoint = new Point(5, 3);
            Point tPoint = new Point(7, 3);
            //Act
            bool result = IsPointOnRight(fPoint, sPoint, tPoint);
            //Assert
            Assert.True(result);
        }

        [Test]
        public void TestIsPointOnRight2()
        {
            //Arrange
            Point fPoint = new Point(1, 1);
            Point sPoint = new Point(1, 10);
            Point tPoint = new Point(0, 10);
            //Act
            bool result = IsPointOnRight(fPoint, sPoint, tPoint);
            //Assert
            Assert.True(result);
        }

        [Test]
        public void TestIsPointOnLeft()
        {
            //Arrange
            Point fPoint = new Point(5, 5);
            Point sPoint = new Point(5, 3);
            Point tPoint = new Point(3, 3);
            //Act
            bool result = IsPointOnRight(fPoint, sPoint, tPoint);
            //Assert
            Assert.False(result);
        }
    }
}
