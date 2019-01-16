using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUPA.Util
{
    public static class RandomInteger
    {
        private static Random rnd = new Random();
        public static int GetRandom()
        {
            return rnd.Next();
        }
    }
}