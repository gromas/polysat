using System;
using System.Text;

namespace PolySat
{
    public class Vector : IRemovable
    {
        private bool removed;
        private readonly VectorStore store;
        private ArraySegment<ulong> vectorData;
        private ArraySegment<ulong> vectorMask;
        private readonly (int x0, int x1, int x2, byte vindex) index;

        public Vector(VectorStore store, (int x0, int x1, int x2, byte vindex) index) :
            this(store, index, new ulong[store.VectorSize], new ulong[store.VectorSize])
        {
            Initialize();
        }

        private void Initialize()
        {
            vectorData = new ulong[store.VectorSize];
            vectorMask = new ulong[store.VectorSize];
            SetBit(index.x0, (index.vindex >> 2) & 1);
            SetBit(index.x1, (index.vindex >> 1) & 1);
            SetBit(index.x2, index.vindex & 1);
        }

        public Vector(VectorStore store, int x, int v)
        {
            this.store = store;
            vectorData = new ulong[store.VectorSize];
            vectorMask = new ulong[store.VectorSize];
            SetBit(x, v);
        }

        private Vector(VectorStore store, (int x0, int x1, int x2, byte vindex) index, ulong[] vectorData, ulong[] vectorMask)
        {
            this.store = store;
            this.index = index;
            this.vectorData = vectorData;
            this.vectorMask = vectorMask;
        }

        private Vector(VectorStore store, ArraySegment<ulong> vectorData, ArraySegment<ulong> vectorMask)
        {
            this.store = store;
            this.vectorData = vectorData;
            this.vectorMask = vectorMask;
        }

        public void SetBit(int x, int value)
        {
            int shift = (x - 1) % 64;
            var index = (x - 1 - shift) / 64;
            vectorMask[index] |= (ulong)1 << shift;
            vectorData[index] |= (ulong)value << shift;
        }

        public int GetBit(int x)
        {
            int shift = (x - 1) % 64;
            var index = (x - 1 - shift) / 64;
            return (int)((vectorData[index] >> shift) & 1);
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < store.VectorSize; i++)
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
            for (int i = 0; i < store.VectorSize; i++)
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
            ulong[] groupmask = new ulong[store.VectorSize];
            ulong[] groupdata = new ulong[store.VectorSize];
            for (int i = 0; i < store.VectorSize; i++)
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
            var group = new Vector(store, groupdata, groupmask);
            return (group, gs);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(store.VariableCount);
            for (int i = 0; i < store.VariableCount; i++)
            {
                int shift = i % 64;
                var index = (i - shift) / 64;
                sb.Append(((vectorMask[index] >> shift) & 1) == 0 ? "x" : ((vectorData[index] >> shift) & 1) == 1 ? "1" : "0");
            }
            return sb.ToString();
        }

        public (int x0, int x1, int x2, byte vindex) Index => index;

        public void Remove()
        {
            removed = true;
            store.Save(this);
        }

        public bool IsRemoved => removed;

        public void Cleanup()
        {
            Initialize();
        }

        void IRemovable.Restore()
        {
            removed = false;
            Initialize();
        }
    }
}
