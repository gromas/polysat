using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PolySat
{
    public class VectorStore
    {
        const int NotSet = 2;
        private readonly int n;
        private readonly byte[] bytes;
        private readonly int vectorSize;
        private readonly StreamWriter w;
        public VectorStore(int n, StreamWriter w)
        {
            this.w = w;
            this.n = n;
            // one byte for each four bytes of vector and one byte for vector state
            vectorSize = (n - n % 4) / 4 + 1;
            // two bits to store each bit of veсtor
            bytes = new byte[8 * vectorSize * n * (n - 1) * (n - 2) / 6];

            Initialize();
        }

        /// <summary>
        /// Precreate combination vectors
        /// </summary>
        private void Initialize()
        {
            // clean all vector states
            for(int p = 0; p < bytes.Length; p++)
            {
                bytes[p] = 0xAA;
            }
            foreach (var c in Combinations)
            {
                var index = GetIndex(c[0], c[1], c[2]);
                for (int i = 0; i < 8; i++)
                {
                    var state = new ArraySegment<byte>(bytes, (index * 8 + i) * vectorSize, vectorSize);
                    SetValue(state, c[0], (i >> 2) & 1);
                    SetValue(state, c[1], (i >> 1) & 1);
                    SetValue(state, c[2], i & 1);
                }
            }
        }

        private void SetValue(ArraySegment<byte> state, int x, int v)
        {
            x -= 1;
            int b = (x - x % 4) / 4;
            int bp = (3 - x % 4) * 2;

            int s0 = 3 << bp;
            int s1 = (state[b] ^ 0xFF) | s0;
            int s2 = (s1 & ((v << bp) ^ 0xFF)) ^ 0xFF;

            state[b] = (byte)s2;
        }

        /// <summary>
        /// Calculates unique index corellated with {a,b,c} of n
        /// </summary>
        private int GetIndex(int a, int b, int c)
        {
            Debug.Assert(0 < a && a < b && b < c && c <= n, "Index out of range");

            var s0 = c - b - 1;
            var s1 = (1 + a - b) * (a + b - 2 * n) / 2;
            var s2 = (a - 1) * (3 * n * n - 3 * n * a - 3 * n + a * a + a) / 6;

            return s0 + s1 + s2;
        }

        /// <summary>
        /// Returns all cobinations 3 of n
        /// </summary>
        public IEnumerable<Combination> Combinations
        {
            get
            {
                for (int x0 = 1; x0 <= n - 2; x0++)
                {
                    for (int x1 = x0 + 1; x1 <= n - 1; x1++)
                    {
                        for (int x2 = x1 + 1; x2 <= n; x2++)
                        {
                            yield return new Combination(x0, x1, x2);
                        }
                    }
                }
            }
        }

        public IEnumerable<Vector> GetVectors(Combination c)
        {
            int index = GetIndex(c[0], c[1], c[2]);

            for (int i = 0; i < 8; i++)
            {
                var state = new ArraySegment<byte>(bytes, (index * 8 + i) * vectorSize, vectorSize);
                var v = new Vector(n, c, state);
                if (!v.IsRemoved) yield return v;
            }
        }

        public IEnumerable<Vector> GetCompatible(Combination c, Vector s)
        {
            var vv = GetVectors(c).ToArray();
            foreach (var v in vv)
            {
                if (v.IsRemoved) continue;
                if (s.IsCompatible(v))
                {
                    yield return v;
                }
            }
        }

        public void AddConstraint(int a, int b, int c)
        {
            var x = Utils.SortByAbs(a, b, c);
            var i = (x[2][1] > 0 ? 1 : 0) + (x[1][1] > 0 ? 2 : 0) + (x[0][1] > 0 ? 4 : 0);
            var index = GetIndex(x[0][0], x[1][0], x[2][0]);

            ArraySegment<byte> state = new ArraySegment<byte>(bytes, (index * 8 + i) * vectorSize, vectorSize);

            w.WriteLine($"{x[0][1]} {x[1][1]} {x[2][1]} 0");
            
            state[state.Count - 1] |= 1;
        }

        /// <summary>
        /// Adds combination states 2 variable constraint (2-CNF clause)
        /// </summary>
        public void AddConstraint(int x0, int x1)
        {
            var x = new int[] { Math.Abs(x0), Math.Abs(x1) };

            for (int x2 = 1; x2 <= n; x2++)
            {
                if (x2 == x[0] || x2 == x[1]) continue;
                AddConstraint(x0, x1, x2);
                AddConstraint(x0, x1, -x2);
            }
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

        public IEnumerable<Vector> GetRemoved(Combination c)
        {
            int index = GetIndex(c[0], c[1], c[2]);
            for (int i = 0; i < 8; i++)
            {
                var state = new ArraySegment<byte>(bytes, (index * 8 + i) * vectorSize, vectorSize);
                var v = new Vector(n, c, state);
                if (v.IsRemoved) yield return v;
            }
        }
    }
}
