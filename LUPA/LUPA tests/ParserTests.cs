using System;
using System.Collections.Generic;
using LUPA;
using LUPA.DataContainers;
using LUPA.Exceptions;
using NUnit.Framework;

namespace LUPA_tests
{
    [TestFixture]
    public class ParserTests
    {
        [Test]
        public void TestParseContourPointValid()
        {
            //Arrange
            string line = "1. 12 250";
            //Act
            Point result = Parser.ParseContourPoint(line);
            //Assert
            Assert.AreEqual(12, result.X);
            Assert.AreEqual(250, result.Y);
        }

        [Test]
        public void TestParseContourPointNoIndex()
        {
            //Arrange
            string line = "12 250";
            try
            {
                //Act
                Point result = Parser.ParseContourPoint(line);
                Assert.Fail();
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Too few arguments", e.Message);
            }
        }

        [Test]
        public void TestParseContourPointCoordinateOutOf()
        {
            //Arrange
            string line = "1. -12 250";
            try
            {
                //Act
                Point result = Parser.ParseContourPoint(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("X coordinate out of range", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseContourPointCoordinateWrongWritten()
        {
            //Arrange
            string line = "1. 1r2 250";
            try
            {
                //Act
                Point result = Parser.ParseContourPoint(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("X position has to be a floating point number", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseKeyPointValid()
        {
            //Arrange
            string line = "1. 12 250 Name";
            //Act
            KeyPoint result = Parser.ParseKeyPoint(line);
            //Assert
            Assert.AreEqual(12, result.X);
            Assert.AreEqual(250, result.Y);
            Assert.AreEqual("Name", result.Name);
        }

        [Test]
        public void TestParseKeyPointNoName()
        {
            //Arrange
            string line = "1. 12 250";
            try
            {
                //Act
                KeyPoint result = Parser.ParseKeyPoint(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Too few arguments", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseKeyPointCoordinateOutOf()
        {
            //Arrange
            string line = "1. 12 601 xyz";
            try
            {
                //Act
                KeyPoint result = Parser.ParseKeyPoint(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Y coordinate out of range", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseKeyPointCoordinateWrongWritten()
        {
            //Arrange
            string line = "1. 12 25q0 xyz";
            try
            {
                //Act
                KeyPoint result = Parser.ParseKeyPoint(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Y position has to be a floating point number", e.Message);
                return;
            }
            Assert.Fail();
        }
        [Test]
        public void TestParseCustomObjectTypeValid()
        {
            //Arrange
            string line = "1. SZKOŁA Nazwa String X double Y double";
            //Act
            CustomObjectType result = Parser.ParseCustomObjectType(line);
            //Assert
            Assert.AreEqual("SZKOŁA", result.Name);
            Assert.AreEqual("Nazwa", result.VariableNames[0]);
            Assert.AreEqual("X", result.VariableNames[1]);
            Assert.AreEqual("Y", result.VariableNames[2]);
            Assert.AreEqual("String", result.VariableTypes[0]);
            Assert.AreEqual("double", result.VariableTypes[1]);
            Assert.AreEqual("double", result.VariableTypes[2]);
        }

        [Test]
        public void TestParseCustomObjectTypeNoVType()
        {
            //Arrange
            string line = "1. SZKOŁA Nazwa String X double Y";
            //Act
            try
            {
                CustomObjectType result = Parser.ParseCustomObjectType(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Incorrect number of arguments - variable does not have name or type", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseCustomObjectTypeNoXCoord()
        {
            //Arrange
            string line = "1. SZKOŁA Nazwa String Y double";
            //Act
            try
            {
                CustomObjectType result = Parser.ParseCustomObjectType(line);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("At least one coordinate is missing in object declaration", e.Message);
                return;
            }
            Assert.Fail();
        }

        [Test]
        public void TestParseCustomObjectInstanceValid1()
        {
            //Arrange
            string line1 = "2. DOM X double Y double L_MIESZKAŃCÓW int";
            string line2 = "2. DOM 4 3 100";
            //Act
            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            double resultX = (double)result.objectProperties[0];
            double resultY = (double)result.objectProperties[1];
            int resultLMieszk = (int)result.objectProperties[2];
            //Assert
            Assert.AreEqual(100, resultLMieszk);
            Assert.AreEqual(4, resultX);
            Assert.AreEqual(3, resultY);
        }

        [Test]
        public void TestParseCustomObjectInstanceValid2()
        {
            //Arrange
            string line1 = "1. SZKOŁA Nazwa String X double Y double";
            string line2 = "1. SZKOŁA \"Szkoła robienia dużych pieniędzy\" 4 1";
            //Act
            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            double resultX = (double)result.objectProperties[1];
            double resultY = (double)result.objectProperties[2];
            string resultNazwa = (string)result.objectProperties[0];
            //Assert
            Assert.AreEqual("\"Szkoła robienia dużych pieniędzy\"", resultNazwa);
            Assert.AreEqual(4, resultX);
            Assert.AreEqual(1, resultY);
        }

        [Test]
        public void TestParseCustomObjectInstanceNotEnoughArgs()
        {
            //Arrange
            string line1 = "2. DOM X double Y double L_MIESZKAŃCÓW int";
            string line2 = "2. DOM 4";

            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            //Act
            try
            {
                CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Too few arguments", e.Message);
                return;
            }
            Assert.Fail();

        }

        [Test]
        public void TestParseCustomObjectInstanceTooManyArgs()
        {
            //Arrange
            string line1 = "2. DOM X double Y double L_MIESZKAŃCÓW int";
            string line2 = "2. DOM 4 4 12 11 22";

            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            //Act
            try
            {
                CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Too many arguments", e.Message);
                return;
            }
            Assert.Fail();

        }

        [Test]
        public void TestParseCustomObjectInstanceWrongTypeOfArgs()
        {
            //Arrange
            string line1 = "2. DOM X double Y double L_MIESZKAŃCÓW int";
            string line2 = "2. DOM 4 marcin 13";

            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            //Act
            try
            {
                CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("2. argument is not double type", e.Message);
                return;
            }
            Assert.Fail();

        }

        [Test]
        public void TestParseCustomObjectInstanceWrongName()
        {
            //Arrange
            string line1 = "2. DOM X double Y double L_MIESZKAŃCÓW int";
            string line2 = "2. DYM 4 3 13";

            CustomObjectType type = Parser.ParseCustomObjectType(line1);
            List<CustomObjectType> types = new List<CustomObjectType>
            {
                type
            };
            //Act
            try
            {
                CustomObjectInstance result = Parser.ParseCustomObject(line2, types);
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Unrecognised object type", e.Message);
                return;
            }
            Assert.Fail();

        }
    }
}
