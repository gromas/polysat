using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace PolySat
{
    /// <summary>
    /// Inmemory state store
    /// </summary>
    public class StateStore
    {
        private readonly StreamWriter writer = StreamWriter.Null;
        private readonly int n;
        private readonly byte[] states;
        public StateStore(int n)
        {
            Debug.Assert(n >= 3, "Minimal 3-SAT problem contains least 3 variables");
            this.n = n;
            states = new byte[n * (n - 1) * (n - 2) / 6];
            Initialize();
        }

        public StateStore(int n, StreamWriter writer) : this(n)
        {
            this.writer = writer;
        }

        private void Initialize()
        {
            // изначально все состояния существуют
            for (int index = 0; index < states.Length; index++)
            {
                states[index] = 0xFF;
            }
        }

        private int GetIndex(int a, int b, int c)
        {
            Debug.Assert(0 < a && a < b && b < c && c <= n, "Index out of range");

            var s0 = c - b - 1;
            var s1 = (1 + a - b) * (a + b - 2 * n) / 2;
            var s2 = (a - 1) * (3 * n * n - 3 * n * a - 3 * n + a * a + a) / 6;

            return s0 + s1 + s2;
        }

        public IEnumerable<CombinationState> GetStates(Combination c)
        {
            int index = GetIndex(c[0], c[1], c[2]);
            byte state = states[index];
            for (int i = 0; i < 8; i++)
            {
                if (((state >> i) & 1) != 0)
                    yield return new CombinationState(c, i);
            }
        }

        /// <summary>
        /// Removes state from stateStore
        /// </summary>
        public byte RemoveState(CombinationState state)
        {
            Combination c = state.Combination;
            int index = GetIndex(c[0], c[1], c[2]);
            byte stateBit = (byte)(1 << state.State);
            states[index] &= (byte)(stateBit ^ 0xFF);

            writer.WriteLine(state);

            return states[index];
        }

        public int VariablesCount => n;
        public int CombinationsCount => states.Length;

        public StreamWriter Writer => writer;
    }
}
