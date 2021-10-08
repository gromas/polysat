using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private readonly StreamWriter writer;

        public CombinationSet(StateStore stateStore, StreamWriter writer)
        {
            this.writer = writer;
            this.stateStore = stateStore;
        }

        /// <summary>
        /// Adds combination states 3 variable constraint (3-CNF clause)
        /// </summary>
        public void AddConstraint(int a, int b, int c)
        {
            var x = Utils.SortByAbs(a, b, c);

            byte s = (byte)(1 << ((x[2][1] > 0 ? 1 : 0) + (x[1][1] > 0 ? 2 : 0) + (x[0][1] > 0 ? 4 : 0)));

            if ((stateStore[x[0][0], x[1][0], x[2][0]] & s) > 0)
            {
                stateStore[x[0][0], x[1][0], x[2][0]] &= (byte)(s ^ 0xFF);
                writer.WriteLine($"{x[0][1]} {x[1][1]} {x[2][1]} 0");
            }
        }

        /// <summary>
        /// Adds combination states 2 variable constraint (2-CNF clause)
        /// </summary>
        public void AddConstraint(int x0, int x1)
        {
            var x = new int[] { Math.Abs(x0), Math.Abs(x1) };

            for (int x2 = 1; x2 <= stateStore.VariablesCount; x2++)
            {
                if (x2 == x[0] || x2 == x[1]) continue;
                AddConstraint(x0, x1, x2);
                AddConstraint(x0, x1, -x2);
            }
        }

        /// <summary>
        /// Removes combination state from stateStore
        /// </summary>
        public void Remove(CombinationState state)
        {
            Combination c = state.Combination;
            byte stateBit = (byte)(1 << state.State);
            stateStore[c[0], c[1], c[2]] &= (byte)(stateBit ^ 0xFF);

            var s = $"{state.LogStates[state.State][0] * c[0]} {state.LogStates[state.State][1] * c[1]} {state.LogStates[state.State][2] * c[2]} 0";

            writer.WriteLine(s);
        }

        /// <summary>
        /// Massive constraints loading
        /// </summary>
        /// <param name=""></param>
        public void AddConstraints(IEnumerable<int[]> literalset)
        {
            foreach (var ls in literalset)
            {
                if (ls.Length == 3)
                {
                    AddConstraint(ls[0], ls[1], ls[2]);
                }
                else if (ls.Length == 2)
                {
                    AddConstraint(ls[0], ls[1]);
                }
                else
                    throw new Exception("literal count must be 2 or 3");
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
