using System;
using System.Collections.Generic;

namespace PolySat
{
    /// <summary>
    /// Represents all combinations 3 of n
    /// Each combination has up to 8 posible states 
    /// Total used memory to represent all combination is n^3-3n^2+2n bytes (one byte per combination)
    /// </summary>
    public static class StateStoreExtensions
    {
        /// <summary>
        /// Adds combination states 3 variable constraint (3-CNF clause)
        /// </summary>
        public static void AddConstraint(this StateStore store, int x0, int x1, int x2)
        {
            var x = Utils.SortByAbs(x0, x1, x2);
            var state = (x[2][1] > 0 ? 0 : 1) + (x[1][1] > 0 ? 0 : 2) + (x[0][1] > 0 ? 0 : 4);
            var c = new Combination(x[2][0], x[1][0], x[0][0]);
            var s = new CombinationState(c, state);

            store.RemoveState(s);
        }

        /// <summary>
        /// Adds combination states 2 variable constraint (2-CNF clause)
        /// </summary>
        public static void AddConstraint(this StateStore store, int x0, int x1)
        {
            var x = new int[] { Math.Abs(x0), Math.Abs(x1) };

            for (int x2 = 1; x2 <= store.VariablesCount; x2++)
            {
                if (x2 == x[0] || x2 == x[1]) continue;
                store.AddConstraint(x0, x1, x2);
                store.AddConstraint(x0, x1, -x2);
            }
        }

        /// <summary>
        /// Massive constraints loading
        /// </summary>
        /// <param name=""></param>
        public static void AddConstraints(this StateStore store, IEnumerable<int[]> literalset)
        {
            foreach (var ls in literalset)
            {
                if (ls.Length == 3)
                {
                    store.AddConstraint(ls[0], ls[1], ls[2]);
                }
                else if (ls.Length == 2)
                {
                    store.AddConstraint(ls[0], ls[1]);
                }
                else
                    throw new Exception("literal count must be 2 or 3");
            }
        }

        /// <summary>
        /// Returns all cobinations 3 of n
        /// </summary>
        public static IEnumerable<Combination> GetAllCombinations(this StateStore store)
        {
            for (int x0 = 1; x0 <= store.VariablesCount - 2; x0++)
            {
                for (int x1 = x0 + 1; x1 <= store.VariablesCount - 1; x1++)
                {
                    for (int x2 = x1 + 1; x2 <= store.VariablesCount; x2++)
                    {
                        yield return new Combination(x0, x1, x2);
                    }
                }
            }
        }
    }
}
