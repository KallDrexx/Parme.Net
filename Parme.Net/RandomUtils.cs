using System;

namespace Parme.Net
{
    public static class RandomUtils
    {
        /// <summary>
        /// Populates the array of floats with random values
        /// </summary>
        /// <param name="random">The random number generator instance to use</param>
        /// <param name="floats">The array of floats to populate</param>
        public static void NextFloats(this Random random, float[] floats)
        {
            for (var x = 0; x < floats.Length; x++)
            {
                floats[x] = (float) random.NextDouble();
            }
        }
    }
}