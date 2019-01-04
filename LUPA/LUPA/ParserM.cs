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

        public Map ParseFile(string inputFilePath)
        {
            ParserState state = ParserState.START;
            int lineCounter = 0;
            Map map = new Map();
            var reader = new StreamReader(inputFilePath);
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

        private CustomObjectInstance ParseCustomObject(string line, List<CustomObjectType> customObjectTypes)
        {
            CustomObjectInstance obj = null;
            return obj;
        }

        private CustomObjectType ParseCustomObjectType(string line)
        {
            CustomObjectType type = null;
            return type;
        }

        private KeyPoint ParseKeyPoint(string line)
        {
            KeyPoint keyPoint = null;
            return keyPoint;
        }

        private Point ParseContourPoint(string line)
        {
            Point point = null;
            return point;
        }
    }
}
