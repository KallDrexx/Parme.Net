namespace Parme.Net
{
    internal class SimdUtils
    {
        /// <summary>
        /// Calculates the nearest number that's a multiple of the given multiple, without going over.  Used for
        /// iterating over simd offsets.
        /// </summary>
        public static int NearestMultiple(int number, int multiple) => ((number - 1) | (multiple - 1)) + 1 - multiple;
    }
}