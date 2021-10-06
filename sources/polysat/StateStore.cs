using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    public class StateStore
    {
        private readonly int n;
        private readonly byte[] state;
        public StateStore(int n)
        {
            Debug.Assert(n >= 3, "Minimal 3-SAT problem contains least 3 variables");
            this.n = n;
            state = new byte[n * (n - 1) * (n - 2) / 6];
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < state.Length; i++)
            {
                state[i] = 255;
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

        public byte this[int a, int b, int c]
        {
            get
            {
                return state[GetIndex(a, b, c)];
            }
            set
            {
                state[GetIndex(a, b, c)] = value;
            }
        }

        public int VariablesCount => n;
        public int CombinationsCount => state.Length;


        /// <summary>
        /// Returns combination position in combination states store
        /// </summary>
        //private int GetIndex(int x0, int x1, int x2)
        //{
        //    Debug.Assert(!(x0 < 1 || x0 >= x1 || x1 >= x2 || x2 > n), "Index out of range");

        //    var s0 = x2 - x1 - 1;
        //    var s1 = (1 + x0 - x1) * (x0 + x1 - 2 * n) / 2;
        //    var s2 = (x0 - 1) * (3 * n * n - 3 * n * x0 - 3 * n + x0 * x0 + x0) / 6;

        //    return s0 + s1 + s2;
        //}
    }
}
