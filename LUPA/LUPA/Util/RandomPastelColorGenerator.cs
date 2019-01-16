using System;
using System.Windows.Media;

namespace LUPA.Util
{
    public class RandomPastelColorGenerator
    {
        private readonly Random _random;

        public RandomPastelColorGenerator()
        {
            // seed the generator with 2 because
            // this gives a good sequence of colors
            const int RandomSeed = 2;
            _random = new Random(RandomSeed);
        }

        /// <summary>
        /// Returns a random pastel brush
        /// </summary>
        /// <returns></returns>
        public SolidColorBrush GetNextBrush()
        {
            SolidColorBrush brush = new SolidColorBrush(GetNext());
            brush.Freeze();

            return brush;
        }

        /// <summary>
        /// Returns a random pastel color
        /// </summary>
        /// <returns></returns>
        public Color GetNext()
        {
            byte[] colorBytes = new byte[3];
            colorBytes[0] = (byte)(_random.Next(128) + 127);
            colorBytes[1] = (byte)(_random.Next(128) + 127);
            colorBytes[2] = (byte)(_random.Next(128) + 127);

            Color color = new Color
            {
                A = 255,
                R = colorBytes[0],
                B = colorBytes[1],
                G = colorBytes[2]
            };

            return color;
        }
    }
}
