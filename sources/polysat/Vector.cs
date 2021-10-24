using System;
using System.Text;

namespace PolySat
{
    public class Vector
    {
        private readonly int n;
        private readonly int vectorSize;
        private readonly ArraySegment<ulong> vectorData;
        private readonly ArraySegment<ulong> vectorMask;
        private readonly (int x0, int x1, int x2, byte vindex) index;
        public Vector(int n, (int x0, int x1, int x2, byte vindex) index)
        {
            this.n = n;
            this.index = index;
            vectorSize = (n - 1 - (n - 1) % 64) / 64 + 1;
            vectorData = new ulong[vectorSize];
            vectorMask = new ulong[vectorSize];

            SetBit(index.x0, (index.vindex >> 2) & 1);
            SetBit(index.x1, (index.vindex >> 1) & 1);
            SetBit(index.x2, index.vindex & 1);
        }

        private void SetBit(int x, int value)
        {
            int shift = (x - 1) % 64;
            var index = (x - 1 - shift) / 64;
            vectorMask[index] |= (ulong)1 << shift;
            vectorData[index] |= (ulong)value << shift;
        }

        // group vector
        private Vector(int n, ArraySegment<ulong> vectorData, ArraySegment<ulong> vectorMask)
        {
            this.n = n;
            vectorSize = (n - 1 - (n - 1) % 64) / 64 + 1;
            this.vectorData = vectorData;
            this.vectorMask = vectorMask;
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < vectorSize; i++)
            {
                // проверяем соответствие битов, установленных в обоих векторах
                ulong groupmask = vectorMask[i] & v.vectorMask[i];
                if (groupmask == 0) continue;
                if ((vectorData[i] & groupmask) != (v.vectorData[i] & groupmask)) return false;
            }
            return true;
        }

        public bool ExtendTo(Vector v)
        {
            bool changed = false;
            for (int i = 0; i < vectorSize; i++)
            {
                ulong d = vectorData[i];
                ulong m = vectorMask[i];
                ulong groupmask = ~vectorMask[i] & v.vectorMask[i];
                vectorData[i] |= v.vectorData[i] & groupmask;
                vectorMask[i] |= groupmask;
                changed |= d != vectorData[i] || m != vectorMask[i];
            }
            return changed;
        }

        /// <summary>
        /// Calculates group vector
        /// </summary>
        /// <param name="vectors"></param>
        /// <returns></returns>
        public (Vector group, int changesCount) Group(Vector[] vectors)
        {
            int gs = 0;
            ulong[] groupmask = new ulong[vectorSize];
            ulong[] groupdata = new ulong[vectorSize];
            for (int i = 0; i < vectorSize; i++)
            {
                ulong mask = ~vectorMask[i];
                ulong d0 = 0;
                ulong d1 = ~d0;
                foreach (var v in vectors)
                {
                    mask &= v.vectorMask[i];
                    d0 |= v.vectorData[i];
                    d1 &= v.vectorData[i];
                }
                groupmask[i] = mask & ~(d0 ^ d1);
                groupdata[i] = d0 & mask;

                if (groupmask[i] > 0) gs++;
            }
            var group = new Vector(n, groupdata, groupmask);
            return (group, gs);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(n);
            for (int i = 0; i < n; i++)
            {
                int shift = i % 64;
                var index = (i - shift) / 64;
                sb.Append(((vectorMask[index] >> shift) & 1) == 0 ? "x" : ((vectorData[index] >> shift) & 1) == 1 ? "1" : "0");
            }
            return sb.ToString();
        }

        public byte Index => index.vindex;
    }
}
