﻿using LUPA.DataContainers;
using LUPA.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace LUPA
{
    public class Parser
    {
        private enum ParserState
        {
            START, CONTOURPOINTS, KEYPOINTS, OBJECTSDEF, OBJECTS
        }

        public static Map ParseFile(string inputFilePath, out List<string> feedback)
        {
            ParserState state = ParserState.START;
            int lineCounter = 0;
            Map map = new Map();
            StreamReader reader = new StreamReader(inputFilePath);
            string line;
            feedback = new List<string>();
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
                        else
                        {
                            throw new ParseFileException("File does not start with proper commentary line");
                        }
                        break;
                    case ParserState.CONTOURPOINTS:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            VerifyContourPoints(map.ContourPoints);
                            state = ParserState.KEYPOINTS;
                        }
                        else
                        {
                            try
                            {
                                map.ContourPoints.Add(ParseContourPoint(line));
                            }
                            catch (ParseLineException e)
                            {
                                feedback.Add(e.Message + " in line " + lineCounter);
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
                            catch (ParseLineException e)
                            {
                                feedback.Add(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                    case ParserState.OBJECTSDEF:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            state = ParserState.OBJECTS;
                            VerifyObjectsDef(map.CustomObjectTypes);
                        }
                        else
                        {
                            try
                            {
                                map.CustomObjectTypes.Add(ParseCustomObjectType(line));
                            }
                            catch (ParseLineException e)
                            {
                                feedback.Add(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                    case ParserState.OBJECTS:
                        if (line.Length > 0 && line[0] == '#')
                        {
                            throw new ParseFileException("File contains more than four comment lines. There should be four lines starting with a hash symbol. Please verify the file.");
                        }
                        else
                        {
                            try
                            {
                                map.CustomObjects.Add(ParseCustomObject(line, map.CustomObjectTypes));
                            }
                            catch (ParseLineException e)
                            {
                                feedback.Add(e.Message + " in line " + lineCounter);
                            }
                        }
                        break;
                }
            }

            return map;
        }

        private static void VerifyObjectsDef(List<CustomObjectType> customObjectTypes)
        {

            bool isNiedzwiedz = false, isDom = false, isSzkola = false;
            foreach(CustomObjectType cot in customObjectTypes)
            {                
                if (cot.Name.Length == 6 && cot.Name.Substring(0, 4) == "SZKO")
                {
                    isSzkola = true;
                }
                if (cot.Name.Length == 10 && cot.Name.Substring(0, 4) == "NIED")
                {
                    isNiedzwiedz = true;
                }
                if (cot.Name == "DOM")
                {
                    bool isLMieszk = false;
                    for (int i = 0; i < cot.VariableNames.Count; i++)
                    {
                        if (cot.VariableNames[i].Length == 13 && cot.VariableNames[i].Substring(0, 8) == "L_MIESZK" && cot.VariableTypes[i] == "int")
                        {
                            isLMieszk = true;
                        }
                    }
                    if (!isLMieszk)
                    {
                        throw new ParseFileException("Object DOM needs to have variable L_MIESZKAŃCÓW with integer type");
                    }
                    isDom = true;
                }
            }
            if(!(isNiedzwiedz && isDom && isSzkola))
            {
                throw new ParseFileException("You need to declare Niedźwiedz, Dom and Szkoła types to run program");
            }
        }

        public static void VerifyContourPoints(List<Point> contourPoints)
        {
            if(contourPoints.Count < 3)
            {
                throw new ParseFileException("You need at least 3 contour points to run program");
            }
            List<LineSegment> contourLines = new List<LineSegment>();
            for (int i = 0; i < contourPoints.Count - 1; i++)
            {
                contourLines.Add(new LineSegment(contourPoints[i], contourPoints[i + 1]));
            }
            contourLines.Add(new LineSegment(contourPoints[contourPoints.Count - 1], contourPoints[0]));

            for (int i = 0; i < contourLines.Count; i++)
            {
                for(int j = 0; j < contourLines.Count; j++)
                {
                    if (i != j)
                    {
                        if(contourLines[i].IsIntersecting(contourLines[j]))
                        {
                            throw new ParseFileException("Contour lines are intersecting, verify your file");
                        }
                    }
                }
            }
        }

        public static CustomObjectInstance ParseCustomObject(string line, List<CustomObjectType> customObjectTypes)
        {
            string[] elements = line.Split();
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new ParseLineException("Line index has to be an integer");
                }
                string name = elements[1];
                CustomObjectType cot = null;
                for (int i = 0; i < customObjectTypes.Count; i++)
                {
                    if (customObjectTypes[i].Name == name)
                    {
                        cot = customObjectTypes[i];
                        break;
                    }
                }
                if (cot == null)
                {
                    throw new ParseLineException("Unrecognised object type");
                }
                object[] objectProperties = new object[cot.VariableNames.Count];
                double x = 0, y = 0;
                bool isStringLoading = false;
                string loadingString = "";
                for (int i = 2, variableCounter = 0; i < elements.Length; i++)
                {
                    switch (cot.VariableTypes[variableCounter])
                    {
                        case "int":
                            if (!int.TryParse(elements[i], out int argI))
                            {
                                throw new ParseLineException(i + ". argument is not integer type");
                            }
                            objectProperties[variableCounter] = argI;
                            variableCounter++;
                            break;

                        case "double":
                            if (!double.TryParse(elements[i], out double argD))
                            {
                                throw new ParseLineException(i + ". argument is not double type");
                            }
                            if (cot.VariableNames[variableCounter] == "X")
                            {
                                x = argD;
                            }
                            else if (cot.VariableNames[variableCounter] == "Y")
                            {
                                y = argD;
                            }
                            objectProperties[variableCounter] = argD;
                            variableCounter++;
                            break;

                        case "float":
                            if (!float.TryParse(elements[i], out float argF))
                            {
                                throw new ParseLineException(i + ". argument is not float type");
                            }
                            objectProperties[variableCounter] = argF;
                            variableCounter++;
                            break;

                        case "string":
                        case "String":
                            if (!isStringLoading && elements[i][0] != '\"')
                            {
                                throw new ParseLineException(i + ". argument does not start with \" symbol when string expected");
                            }
                            else if (!isStringLoading && elements[i][0] == '\"' && elements[i][elements[i].Length - 1] != '\"')
                            {
                                isStringLoading = true;
                                loadingString = loadingString + elements[i];
                            }
                            else if (isStringLoading && elements[i][elements[i].Length-1] != '\"')
                            {
                                loadingString = loadingString + " " + elements[i];
                            }
                            else if (isStringLoading && elements[i][elements[i].Length - 1] == '\"')
                            {
                                loadingString = loadingString + " " + elements[i];
                                isStringLoading = false;
                                objectProperties[variableCounter] = loadingString;
                                loadingString = "";
                                variableCounter++;
                            }
                            else
                            {
                                objectProperties[variableCounter] = elements[i];
                                variableCounter++;
                            }
                            break;

                        case "bool":
                            if (!bool.TryParse(elements[i], out bool argB))
                            {
                                throw new ParseLineException(i + ". argument is not boolean type");
                            }
                            objectProperties[variableCounter] = argB;
                            variableCounter++;
                            break;

                        case "long":
                            if (!long.TryParse(elements[i], out long argL))
                            {
                                throw new ParseLineException(i + ". argument is not integer type");
                            }
                            objectProperties[variableCounter] = argL;
                            variableCounter++;
                            break;
                    }
                }
                return new CustomObjectInstance(x, y, cot, objectProperties);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParseLineException("Too few arguments");
            }

        }

        public static CustomObjectType ParseCustomObjectType(string line)
        {
            string[] elements = line.Split();
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new ParseLineException("Line index has to be an integer");
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
                        catch (ObjectTypeDeclarationException e)
                        {
                            throw new ParseLineException(e.Message);
                        }
                    }
                    else
                    {
                        throw new ParseLineException("Incorrect number of arguments - variable does not have name or type");
                    }
                }
                return cot;
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParseLineException("Too few arguments");
            }
        }

        public static KeyPoint ParseKeyPoint(string line)
        {
            string[] elements = line.Split();
            string name = "";
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new ParseLineException("Line index has to be an integer");
                }
                if (!double.TryParse(elements[1], out double x))
                {
                    throw new ParseLineException("X position has to be a floating point number");
                }
                if (!double.TryParse(elements[2], out double y))
                {
                    throw new ParseLineException("Y position has to be a floating point number");
                }
                for (int i = 3; i < elements.Length; i++)
                {
                    name += elements[i];
                }
                return new KeyPoint(x, y, name);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParseLineException("Too few arguments");
            }
        }

        public static Point ParseContourPoint(string line)
        {
            string[] elements = line.Split();
            if (elements.Length > 3)
            {
                throw new ParseLineException("Too many arguments");
            }
            try
            {
                if (!int.TryParse(elements[0].Substring(0, elements[0].Length - 1), out int index))
                {
                    throw new ParseLineException("Line index has to be an integer");
                }
                if (!double.TryParse(elements[1], out double x))
                {
                    throw new ParseLineException("X position has to be a floating point number");
                }
                if (!double.TryParse(elements[2], out double y))
                {
                    throw new ParseLineException("Y position has to be a floating point number");
                }
                return new Point(x, y);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ParseLineException("Too few arguments");
            }
        }
    }
}
