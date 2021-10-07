using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PolySat
{
    /// <summary>
    /// Represents all combinations 3 of n
    /// Each combination has up to 8 posible states 
    /// Total used memory to represent all combination is n^3-3n^2+2n bytes (one byte per combination)
    /// </summary>
    public class CombinationSet
    {
        // combination states inmemory store
        private readonly StateStore stateStore;

        public CombinationSet(StateStore stateStore)
        {
            this.stateStore = stateStore;
        }

        /// <summary>
        /// Adds combination states 3 variable constraint (3-CNF clause)
        /// </summary>
        public void AddConstraint(int a, int b, int c)
        {
            var x = new int[] { a, b, c }.OrderBy(x => Math.Abs(x)).ToArray();
            stateStore[Math.Abs(x[0]), Math.Abs(x[1]), Math.Abs(x[2])] &= (byte)((1 << ((x[2] > 0 ? 1 : 0) + (x[1] > 0 ? 2 : 0) + (x[0] > 0 ? 4 : 0))) ^ 0xFF);
        }

        /// <summary>
        /// Adds combination states 2 variable constraint (2-CNF clause)
        /// </summary>
        public void AddConstraint(int x0, int x1)
        {
            for (int x2 = 1; x2 <= stateStore.VariablesCount; x2++)
            {
                if (Math.Abs(x2) == Math.Abs(x0) || Math.Abs(x2) == Math.Abs(x1)) continue;
                AddConstraint(x0, x1, x2);
                AddConstraint(x0, x1, -x2);
            }
        }

        /// <summary>
        /// Returns combination state
        /// </summary>
        /// <param name="x">combination</param>
        /// <returns>byte represents combination states</returns>
        public byte this[Combination x]
        {
            get
            {
                return stateStore[x[0], x[1], x[2]];
            }
        }

        /// <summary>
        /// Returns all cobinations 3 of n
        /// </summary>
        public IEnumerable<Combination> All
        {
            get
            {
                for (int x0 = 1; x0 <= stateStore.VariablesCount - 2; x0++)
                    for (int x1 = x0 + 1; x1 <= stateStore.VariablesCount - 1; x1++)
                        for (int x2 = x1 + 1; x2 <= stateStore.VariablesCount; x2++)
                        {
                            yield return new Combination(x0, x1, x2);
                        }
            }
        }

        public IEnumerable<CombinationState> PosibleStates(Combination c)
        {
            byte state = stateStore[c[0],c[1],c[2]];
            for (int i = 0; i < 8; i++)
            {
                if ((state & (1 << i)) == 0) continue;
                yield return new CombinationState(c, i);
            }
        }

        public int VariablesCount => stateStore.VariablesCount;
    }
}
