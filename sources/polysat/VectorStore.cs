using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace PolySat
{
    public class VectorStore
    {
        private readonly int n;
        private readonly int combinationsCount;
        private readonly int vectorSize;
        private readonly uint[] vectordata;
        private readonly uint[] vectormask;
        private readonly BitArray removed;

        public VectorStore(int n)
        {
            this.n = n;
            // total combinations count
            combinationsCount = n * (n - 1) * (n - 2) / 6;
            
            vectorSize = (n - 1 - (n - 1) % 32) / 32 + 1;
            
            vectordata = new uint[combinationsCount * 8 * vectorSize];
            // vector mask 0 - bit not set; 1 - bit set
            vectormask = new uint[combinationsCount * 8 * vectorSize];
            // one bit to store each vector REMOVED state
            removed = new BitArray(8 * combinationsCount);

            Initialize();
        }

        /// <summary>
        /// Precreate combination vectors
        /// </summary>
        private void Initialize()
        {
            foreach (Tuple<int, int, int> ct in CombinationsTuples)
            {
                for (int i = 0; i < 8; i++)
                {
                    var vector = this[ct, i];
                    vector.SetBit(ct.Item1, (i >> 2) & 1);
                    vector.SetBit(ct.Item2, (i >> 1) & 1);
                    vector.SetBit(ct.Item3, i & 1);
                }
            }
        }

        public Vector this[int index] => 
            new Vector(this, index,
                new ArraySegment<uint>(vectordata, index * vectorSize, vectorSize),
                new ArraySegment<uint>(vectormask, index * vectorSize, vectorSize));

        public Vector this[Tuple<int, int, int> combinationIndex, int vectorIndex] => 
            this[GetIndex(combinationIndex) * 8 + vectorIndex];

        /// <summary>
        /// Calculates unique index corellated with {a,b,c} of n
        /// </summary>
        private int GetIndex(Tuple<int, int, int> x)
        {
            Debug.Assert(x.Item1 < x.Item2 && x.Item2 < x.Item3 && x.Item3 <= n, "Index out of range");

            int s0 = x.Item3 - x.Item2 - 1;
            int s1 = (1 + x.Item1 - x.Item2) * (x.Item1 + x.Item2 - 2 * n) / 2;
            int s2 = (x.Item1 - 1) * (3 * n * n - 3 * n * x.Item1 - 3 * n + x.Item1 * x.Item1 + x.Item1) / 6;

            return s0 + s1 + s2;
        }

        public IEnumerable<Combination> Combinations
        {
            get
            {
                for (int index = 0; index < combinationsCount; index++)
                {
                    yield return new Combination(this, index);
                }
            }
        }

        /// <summary>
        /// Returns all cobinations 3 of n
        /// </summary>
        private IEnumerable<Tuple<int, int, int>> CombinationsTuples
        {
            get
            {
                for (int x0 = 1; x0 <= n - 2; x0++)
                    for (int x1 = x0 + 1; x1 <= n - 1; x1++)
                        for (int x2 = x1 + 1; x2 <= n; x2++)
                            yield return new Tuple<int, int, int>(x0, x1, x2);
            }
        }

        public void AddConstraint(int a, int b, int c)
        {
            var x = Utils.SortByAbs(a, b, c);
            var i = (x[2][1] > 0 ? 1 : 0) + (x[1][1] > 0 ? 2 : 0) + (x[0][1] > 0 ? 4 : 0);
            var index = GetIndex(new Tuple<int, int, int>(x[0][0], x[1][0], x[2][0]));
            RemoveVector(index * 8 + i);
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
        public int VariablesCount => n;

        /// <summary>
        /// Total n by 3 combinations count
        /// </summary>
        public int CombinationCount => combinationsCount;

        /// <summary>
        /// Vector size in bytes
        /// </summary>
        public int VectorSize => vectorSize;
        public bool VectorRemoved(int index) => removed[index];
        public void RemoveVector(int index) => removed[index] = true;
        
    }
}
