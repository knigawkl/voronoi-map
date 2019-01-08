using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            }
            catch (ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Too few arguments", e.Message);
            }
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
            }
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
            catch(ParseLineException e)
            {
                //Assert
                Assert.AreEqual("Incorrect number of arguments - variable does not have name or type", e.Message);
            }       
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
            int resultLMieszk = (int) result.objectProperties[2];
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
    }
}
