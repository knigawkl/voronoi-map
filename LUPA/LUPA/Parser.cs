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
            public int Id { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
        }

        public List<CountourPoint> countour;
        public int width = 0;
        public int height = 0;

        public void Parse(string inputFilePath)
        {
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

                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }

            }
            catch (Exception e)
            {
                //wypisac do konsolki w okienku blad
            }

        }
    }
}

