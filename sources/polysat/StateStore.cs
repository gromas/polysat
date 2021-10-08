using System.Diagnostics;

namespace PolySat
{
    /// <summary>
    /// Inmemory state store
    /// </summary>
    public class StateStore
    {
        private readonly int n;
        private readonly byte[] state;
        public StateStore(int n)
        {
            Debug.Assert(n >= 3, "Minimal 3-SAT problem contains least 3 variables");
            this.n = n;
            state = new byte[n * (n - 1) * (n - 2) / 6];
        }

        private int GetIndex(int a, int b, int c)
        {
            Debug.Assert(0 < a && a < b && b < c && c <= n, "Index out of range");

            var s0 = c - b - 1;
            var s1 = (1 + a - b) * (a + b - 2 * n) / 2;
            var s2 = (a - 1) * (3 * n * n - 3 * n * a - 3 * n + a * a + a) / 6;

            return s0 + s1 + s2;
        }

        public byte this[int a, int b, int c]
        {
            get
            {
                return (byte)(state[GetIndex(a, b, c)] ^ 0xFF);
            }
            set
            {
                state[GetIndex(a, b, c)] = (byte)(value ^ 0xFF);
            }
        }

        public int VariablesCount => n;
        public int CombinationsCount => state.Length;
    }
}
