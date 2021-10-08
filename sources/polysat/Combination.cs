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
            x = Utils.Sort(x0, x1, x2);
        }

        public int this[int index]
        {
            get
            {
                return x[index];
            }
        }

        public IEnumerable<CombinationState> GetStates(CombinationSet set)
        {
            var state = set[this];
            for (int i = 0; i < 8; i++)
            {
                if ((state & 1) == 1)
                {
                    yield return new CombinationState(this, i);
                }
                state >>= 1;
            }
        }
    }
}
