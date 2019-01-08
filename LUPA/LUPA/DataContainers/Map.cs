using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.DataContainers
{
    public class Map
    {
        public List<Point> ContourPoints { set; get; }
        public List<KeyPoint> KeyPoints { set; get; }
        public List<LineSegment> AreaLineSegments { set; get; }
        public List<CustomObjectType> CustomObjectTypes { set; get; }
        public List<CustomObjectInstance> CustomObjects { set; get; }

        public Map()
        {
            ContourPoints = new List<Point>();
            KeyPoints = new List<KeyPoint>();
            AreaLineSegments = new List<LineSegment>();
            CustomObjectTypes = new List<CustomObjectType>();
            CustomObjects = new List<CustomObjectInstance>();
        }
    }
}
