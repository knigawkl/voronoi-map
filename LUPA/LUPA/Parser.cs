using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA
{
    public class Parser
    {
        public struct CountourPoint
        {
            public string Id { get; set; }
            public double X { get; set; }
            public double Y { get; set; }

            public CountourPoint(string id, double x, double y)
            {
                Id = id;
                X = x;
                Y = y;
            }
        }

        
        public int width = 0;
        public int height = 0;

        public List<CountourPoint> ParseContour(string inputFilePath)
        {
            List<CountourPoint> contour = new List<CountourPoint>();
            try
            {
                var lines = new List<string>();
                using (var reader = new StreamReader(inputFilePath))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        lines.Add(line);
                    }
                }

                int hashCounter = 0;
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        hashCounter++;
                    }
                }
                if (hashCounter != 4)
                {
                    throw new Exception("There should be four comment lines, each starting with a hash symbol. " +
                        "Currently there are " + hashCounter + " such lines");
                }

                int[] commentLinesIndices = new int[4];
                int lineNumIterator = 0, commentLineIterator = 0;
                foreach (var line in lines)
                {
                    if (line.StartsWith("#"))
                    {
                        commentLinesIndices[commentLineIterator] = lineNumIterator;
                        commentLineIterator++;
                    }
                    lineNumIterator++;
                }

                
                for (int i = commentLinesIndices[0] + 1; i < commentLinesIndices[1] - 1; i++)
                {
                    string[] elems = lines[i].Split();
                    string index = elems[0];
                    if (!double.TryParse(elems[1], out var xPos))
                    {
                        throw new Exception("X position has to be a floating point number. Line: " + (i + 1));
                    }
                    if (!double.TryParse(elems[2], out var yPos))
                    {
                        throw new Exception("Y position has to be a floating point number. Line: " + (i + 1));
                    }

                    contour.Add(new CountourPoint(index, xPos, yPos));
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "Lukaszku" + e.StackTrace);
            }
            return contour;
        }
    }
}
