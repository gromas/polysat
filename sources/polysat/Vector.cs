using PolySat.Solver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    /// <summary>
    /// Вектор назначения цепочки сочетаний
    /// </summary>
    public class Vector
    {
        private readonly int n;
        public readonly Combination c;
        public readonly byte vIndex;
        private readonly ArraySegment<ulong> bitSet0;
        private readonly ArraySegment<ulong> bitSet1;

        internal Vector(int n, Combination c, byte vIndex, ArraySegment<ulong> bitSet0, ArraySegment<ulong> bitSet1)
        {
            this.n = n;
            this.c = c;
            this.vIndex = vIndex;
            this.bitSet0 = bitSet0;
            this.bitSet1 = bitSet1;
        }
        /// <summary>
        /// Назначает переменной x значение value
        /// </summary>
        /// <param name="x"></param>
        /// <param name="value"></param>
        internal void SetBit(int x, int value)
        {
            int shift = (x - 1) % 64;
            var index = (x - 1 - shift) / 64;
            switch (value)
            {
                case 0:
                    bitSet0[index] |= (ulong)1 << shift;
                    break;
                case 1:
                    bitSet1[index] |= (ulong)1 << shift;
                    break;
            }
        }

        public IEnumerable<Vector> GetCompatible(IEnumerable<Vector> vs)
        {
            return vs.Where(v => IsCompatible(v));
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < bitSet0.Count; i++)
            {
                if ((bitSet0[i] & v.bitSet1[i]) > 0) return false;
                if ((bitSet1[i] & v.bitSet0[i]) > 0) return false;
            }
            return true;
        }

        public static (int n, Combination c, ulong[] bitSet0, ulong[] bitSet1) Aggregate((int n, Combination c, ulong[] bitSet0, ulong[] bitSet1) acc, Vector outer)
        {
            if (acc.n == 0)
            {
                acc.n = outer.n;
                acc.bitSet0 = new ulong[outer.bitSet0.Count];
                acc.bitSet1 = new ulong[outer.bitSet1.Count];
                acc.c = outer.c;
                outer.bitSet0.AsSpan().CopyTo(acc.bitSet0.AsSpan());
                outer.bitSet1.AsSpan().CopyTo(acc.bitSet1.AsSpan());
                return acc;
            }
            for (int i = 0; i < acc.bitSet0.Length; i++)
            {
                acc.bitSet0[i] &= outer.bitSet0[i];
                acc.bitSet1[i] &= outer.bitSet1[i];
            }
            return acc;
        }

        public static Vector Group(IEnumerable<Vector> source)
        {
            var vector = source.Aggregate<Vector, (int n, Combination c, ulong[] bitSet0, ulong[] bitSet1), Vector>
                ((0, new Combination { }, null, null), Aggregate, acc => new Vector(acc.n, acc.c, 255, acc.bitSet0, acc.bitSet1));
            return vector;
        }

        public bool Apply(Vector v)
        {
            for (int i = 0; i < bitSet0.Count; i++)
            {
                if (((bitSet0[i] | v.bitSet0[i]) ^ bitSet0[i] ^ (bitSet1[i] | v.bitSet1[i]) ^ bitSet1[i]) != 0)
                {
                    for (; i < bitSet0.Count; i++)
                    {
                        bitSet0[i] |= v.bitSet0[i];
                        bitSet1[i] |= v.bitSet1[i];
                    }
                    return true;
                }
            }
            return false;
        }

        internal VectorSnapshot Snapshot()
        {
            return new VectorSnapshot(bitSet0, bitSet1);
        }

        internal bool Restore(VectorSnapshot snapshot)
        {
            bool changed = !snapshot.bitSet0.AsSpan().SequenceEqual(bitSet0.AsSpan()) ||
                           !snapshot.bitSet1.AsSpan().SequenceEqual(bitSet1.AsSpan());
            if (changed)
            {
                snapshot.bitSet0.AsSpan().CopyTo(bitSet0.AsSpan());
                snapshot.bitSet1.AsSpan().CopyTo(bitSet1.AsSpan());
            }
            return changed;
        }

        public override string ToString()
        {
            StringBuilder sb = new(n);
            for (int i = 0; i < n; i++)
            {
                int shift = i % 64;
                var index = (i - shift) / 64;
                sb.Append(((bitSet0[index] >> shift) & 1) == 1 ? "0" : ((bitSet1[index] >> shift) & 1) == 1 ? "1" : "x");
            }
            return sb.ToString();
        }
    }
}
