using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    /// <summary>
    /// Combination: represents an combination 3 of n
    /// </summary>
    public class Combination
    {
        private readonly int[] x;
        public Combination(int x0, int x1, int x2)
        {
            x = new int[] { x0, x1, x2 }.OrderBy(x => x).ToArray();
        }

        public int this[int index]
        {
            get
            {
                return x[index];
            }
        }
    }
}
