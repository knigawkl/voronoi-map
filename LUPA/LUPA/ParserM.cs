using LUPA.DataContainers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA
{
    class ParserM
    {
        private enum ParserState
        {
            START, CONTOURPOINTS, KEYPOINTS, OBJECTSDEF, OBJECTS
        }

        public static Map ParseFile(string inputFilePath)
        {
            ParserState state = ParserState.START;
            int lineCounter = 0;
            Map map = new Map();
            StreamReader reader = new StreamReader(inputFilePath);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                lineCounter++;
                switch (state)
                {
                    case ParserState.START:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            state = ParserState.CONTOURPOINTS;                          
                        }
                        break;
                    case ParserState.CONTOURPOINTS:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            state = ParserState.KEYPOINTS;
                        }
                        else
                        {
                            try
                            {
                                map.ContourPoints.Add(ParseContourPoint(line));
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                    case ParserState.KEYPOINTS:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            state = ParserState.OBJECTSDEF;
                        }
                        else
                        {
                            try
                            {
                                map.KeyPoints.Add(ParseKeyPoint(line));
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                    case ParserState.OBJECTSDEF:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            state = ParserState.OBJECTS;
                        }
                        else
                        {
                            try
                            {
                                map.CustomObjectTypes.Add(ParseCustomObjectType(line));
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                    case ParserState.OBJECTS:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            throw new Exception("File contains more than four comment lines. There should be four lines starting with a hash symbol. Please verify the file.");
                        }
                        else
                        {
                            try
                            {
                                map.CustomObjects.Add(ParseCustomObject(line, map.CustomObjectTypes));
                            }
                            catch (Exception e)
                            {
                                throw new Exception(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                }
            }

            return map;
        }

        private static CustomObjectInstance ParseCustomObject(string line, List<CustomObjectType> customObjectTypes)
        {
            string[] elements = line.Split();
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new Exception("Line index has to be an integer");
                }
                string name = elements[1];
                CustomObjectType cot = null;
                for (int i = 0; i < customObjectTypes.Capacity; i++)
                {
                    if(customObjectTypes[i].Name == name)
                    {
                        cot = customObjectTypes[i];
                        break;
                    }
                }
                if(cot == null)
                {
                    throw new Exception("Unrecognised object type");
                }
                string [] args = new string[elements.Length - 2];
                for(int i = 2; i < elements.Length; i++)
                {
                    args[i - 2] = elements[i];
                }
                try
                {              
                    return new CustomObjectInstance(cot, args);
                }
                catch(Exception e)
                {
                    throw e;
                }
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Too few arguments");
            }
            
        }

        private static CustomObjectType ParseCustomObjectType(string line)
        {
            string[] elements = line.Split();
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new Exception("Line index has to be an integer");
                }
                string name = elements[1];
                CustomObjectType cot = new CustomObjectType(name);
                for (int i = 2; i < elements.Length; i++)
                {
                    string variableName = elements[i++];
                    if (i < elements.Length)
                    {
                        string variableType = elements[i];
                        try
                        {
                            cot.AddVariable(variableName, variableType);
                        }
                        catch(Exception e)
                        {
                            throw e;
                        }
                    }
                    else
                    {
                        throw new Exception("Incorrect number of arguments - variable does not have name or type");
                    }
                }
                return cot;
            }
            catch (IndexOutOfRangeException)
            {
                throw new Exception("Too few arguments");
            }
        }

        private static KeyPoint ParseKeyPoint(string line)
        {
            string[] elements = line.Split();
            string name = "";
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new Exception("Line index has to be an integer");
                }
                if (!double.TryParse(elements[1], out double x))
                {
                    throw new Exception("X position has to be a floating point number");
                }
                if (!double.TryParse(elements[2], out double y))
                {
                    throw new Exception("Y position has to be a floating point number");
                }
                for (int i = 3; i < elements.Length; i++)
                {
                    name += elements[i];
                }
                return new KeyPoint(x, y, name);
            }
            catch(IndexOutOfRangeException)
            {
                throw new Exception("Too few arguments");
            }                     
        }

        private static Point ParseContourPoint(string line)
        {
            string[] elements = line.Split();
            if(elements.Length > 3)
            {
                throw new Exception("Too many arguments");
            }
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new Exception("Line index has to be an integer");
                }
                if (!double.TryParse(elements[1], out double x))
                {
                    throw new Exception("X position has to be a floating point number");
                }
                if (!double.TryParse(elements[2], out double y))
                {
                    throw new Exception("Y position has to be a floating point number");
                }
                return new Point(x, y);
            }
            catch(IndexOutOfRangeException)
            {
                throw new Exception("Too few arguments");
            }
        }
    }
}
