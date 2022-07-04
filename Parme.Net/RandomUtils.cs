using System;

namespace Parme.Net
{
    public static class RandomUtils
    {
        /// <summary>
        /// Populates the array of doubles with random values
        /// </summary>
        /// <param name="random">The random number generator instance to use</param>
        /// <param name="doubles">The array of doubles to populate</param>
        public static void NextDoubles(this Random random, double[] doubles)
        {
            for (var x = 0; x < doubles.Length; x++)
            {
                doubles[x] = random.NextDouble();
            }
        }
    }
}