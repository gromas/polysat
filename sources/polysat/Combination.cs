using System;
using System.Collections.Generic;

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

        public override string ToString()
        {
            return $"{{{x[0]},{x[1]},{x[2]}}}";
        }

        public bool Equals(Combination other)
        {
            return other.x[0] == x[0] && other.x[1] == x[1] && other.x[2] == x[2];
        }
    }
}
