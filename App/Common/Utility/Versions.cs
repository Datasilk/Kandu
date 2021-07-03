namespace Kandu.Common.Utility
{
    public static class Versions
    {
        /// <summary>
        /// Checks to see if v1 > v2
        /// </summary>
        /// <param name="v1">New version. Must be 4 integers.</param>
        /// <param name="v2">Old version. Must be 4 integers.</param>
        /// <returns></returns>
        public static bool Compare(int[] v1, int[] v2, bool canEqual = false)
        {
            return 
                (
                    canEqual &&
                    v1[0] == v2[0] &&
                    v1[1] == v2[1] &&
                    v1[2] == v2[2] &&
                    v1[3] == v2[3]
                ) || 
                (
                    v1[0] > v2[0] || (
                    v1[0] == v2[0] &&
                    (v1[1] > v2[1] || (
                    v1[1] == v2[1] &&
                    (v1[2] > v2[2] || (
                    v1[2] == v2[2] &&
                    (v1[3] > v2[3]
                    ))))))
                );
        }
    }
}
