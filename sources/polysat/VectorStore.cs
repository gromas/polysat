using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PolySat
{
    public class VectorStore
    {
        const byte NotSet = 0xAA;
        private readonly byte[] bytes;
        private readonly uint n;
        private readonly uint combinationsCount;
        private readonly uint vectorSize;
        private readonly BitArray removed;

        public VectorStore(uint n)
        {
            this.n = n;
            //
            combinationsCount = n * (n - 1) * (n - 2) / 6;
            // one byte for each four bytes of vector and one byte for vector state
            vectorSize = (n - n % 4) / 4 + 1;
            // two bits to store each bit of veсtor
            bytes = new byte[8 * vectorSize * combinationsCount];


            removed = new BitArray((int)(8 * combinationsCount));

            Initialize();
        }

        /// <summary>
        /// Precreate combination vectors
        /// </summary>
        private void Initialize()
        {
            // clean all vector states
            for (int p = 0; p < bytes.Length; p++)
            {
                bytes[p] = NotSet;
            }
            foreach (Tuple<uint, uint, uint> ct in CombinationsTuples)
            {
                uint index = GetIndex(ct);
                for (uint i = 0; i < 8; i++)
                {
                    var vector = new Vector(this, index * 8 + i);
                    vector.SetValue(ct.Item1, (int)(i >> 2) & 1);
                    vector.SetValue(ct.Item2, (int)(i >> 1) & 1);
                    vector.SetValue(ct.Item3, (int)i & 1);
                }
            }
        }

        /// <summary>
        /// Calculates unique index corellated with {a,b,c} of n
        /// </summary>
        private uint GetIndex(Tuple<uint, uint, uint> x)
        {
            Debug.Assert(x.Item1 < x.Item2 && x.Item2 < x.Item3 && x.Item3 <= n, "Index out of range");

            uint s0 = x.Item3 - x.Item2 - 1;
            uint s1 = (1 + x.Item1 - x.Item2) * (x.Item1 + x.Item2 - 2 * n) / 2;
            uint s2 = (x.Item1 - 1) * (3 * n * n - 3 * n * x.Item1 - 3 * n + x.Item1 * x.Item1 + x.Item1) / 6;

            return s0 + s1 + s2;
        }

        public IEnumerable<Combination> Combinations
        {
            get
            {
                for (uint index = 0; index < combinationsCount; index++)
                {
                    yield return new Combination(this, index);
                }
            }
        }

        /// <summary>
        /// Returns all cobinations 3 of n
        /// </summary>
        private IEnumerable<Tuple<uint, uint, uint>> CombinationsTuples
        {
            get
            {
                for (uint x0 = 1; x0 <= n - 2; x0++)
                    for (uint x1 = x0 + 1; x1 <= n - 1; x1++)
                        for (uint x2 = x1 + 1; x2 <= n; x2++)
                            yield return new Tuple<uint, uint, uint>(x0, x1, x2);
            }
        }

        public void AddConstraint(int a, int b, int c)
        {
            var x = Utils.SortByAbs(a, b, c);
            var i = (x[2][1] > 0 ? 1 : 0) + (x[1][1] > 0 ? 2 : 0) + (x[0][1] > 0 ? 4 : 0);
            var index = GetIndex(new Tuple<uint, uint, uint>((uint)x[0][0], (uint)x[1][0], (uint)x[2][0]));
            new Vector(this, (uint)(index * 8 + i)).IsRemoved = true;
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

        /// <summary>
        /// Variables count n
        /// </summary>
        public uint VariablesCount => n;

        /// <summary>
        /// Total n by 3 combinations count
        /// </summary>
        public uint CombinationCount => combinationsCount;

        /// <summary>
        /// Vector size in bytes
        /// </summary>
        public uint VectorSize => vectorSize;

        /// <summary>
        /// Returns full storage byteset
        /// </summary>
        public byte[] Bytes => bytes;
        public bool IsRemoved(uint index) => removed[(int)index];
        public void Remove(uint index) => removed[(int)index] = true;
        
    }
}
