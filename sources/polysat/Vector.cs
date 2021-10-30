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
        private readonly ArraySegment<ulong> data;
        private readonly ArraySegment<ulong> mask;

        internal Vector(int n, Combination c, byte vIndex, ArraySegment<ulong> data, ArraySegment<ulong> mask)
        {
            this.n = n;
            this.c = c;
            this.vIndex = vIndex;
            this.data = data;
            this.mask = mask;
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
            mask[index] |= (ulong)1 << shift;
            data[index] |= (ulong)value << shift;
        }

        public IEnumerable<Vector> GetCompatible(IEnumerable<Vector> vs)
        {
            return vs.Where(v => IsCompatible(v));
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < mask.Count; i++)
            {
                // проверяем соответствие битов, установленных в обоих векторах
                ulong groupmask = mask[i] & v.mask[i];
                if (groupmask == 0) continue;
                if (((data[i] & groupmask) ^ (v.data[i] & groupmask)) > 0) return false;
            }
            return true;
        }

        public (Vector group, int changesCount) Group(IEnumerable<Vector> vectors)
        {
            int gs = 0;
            ulong[] groupmask = new ulong[mask.Count];
            ulong[] groupdata = new ulong[mask.Count];
            for (int i = 0; i < mask.Count; i++)
            {
                ulong mask = ~this.mask[i];
                ulong d0 = 0;
                ulong d1 = ~d0;
                foreach (var v in vectors)
                {
                    mask &= v.mask[i];
                    d0 |= v.data[i];
                    d1 &= v.data[i];
                }
                groupmask[i] = mask & ~(d0 ^ d1);
                groupdata[i] = d0 & mask;

                if (groupmask[i] > 0) gs++;
            }
            var group = new Vector(n, c, 0xff, groupdata, groupmask);
            return (group, gs);
        }

        public bool Apply(Vector v)
        {
            bool changed = false;
            for (int i = 0; i < mask.Count; i++)
            {
                ulong d = data[i];
                ulong m = mask[i];
                ulong groupmask = ~mask[i] & v.mask[i];
                data[i] |= v.data[i] & groupmask;
                mask[i] |= groupmask;
                changed |= d != data[i] || m != mask[i];
            }
            return changed;
        }

        internal VectorSnapshot Snapshot()
        {
            return new VectorSnapshot(data, mask);
        }

        internal bool Restore(VectorSnapshot snapshot)
        {
            bool changed = !snapshot.mask.AsSpan().SequenceEqual(mask.AsSpan());
            if (changed)
            {
                snapshot.data.AsSpan().CopyTo(data.AsSpan());
                snapshot.mask.AsSpan().CopyTo(mask.AsSpan());
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
                sb.Append(((mask[index] >> shift) & 1) == 0 ? "x" : ((data[index] >> shift) & 1) == 1 ? "1" : "0");
            }
            return sb.ToString();
        }
    }
}
