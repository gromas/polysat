using System;
using System.Text;

namespace PolySat
{
    public class Vector
    {
        private readonly VectorStore store;
        private readonly int index;
        private readonly ArraySegment<uint> data;
        private readonly ArraySegment<uint> mask;
        public Vector(VectorStore store, int index, ArraySegment<uint> data, ArraySegment<uint> mask)
        {
            this.store = store;
            this.index = index;
            this.data = data;
            this.mask = mask;
        }

        public void SetBit(int x, int value)
        {
            int shift = (x - 1) % 32;
            var index = (x - 1 - shift) / 32;
            mask[index] |= (uint)(1 << shift);
            data[index] |= (uint)(value << shift);
        }

        public (Vector, int) Group(Vector[] vectors)
        {
            int gs = 0;
            uint[] groupmask = new uint[mask.Count];
            uint[] groupdata = new uint[mask.Count];
            // create group vector outside store
            for (int i = 0; i < mask.Count; i++)
            {
                // считаем маску группы
                groupdata[i] = 0;
                groupmask[i] = ~mask[i];
                foreach (var v in vectors)
                {
                    // считаем текущую маску группы
                    groupmask[i] &= v.mask[i];
                    // сбрасываем ранее установленные биты, если маска совместимости уменьшилась
                    groupdata[i] &= groupmask[i];
                    // считаем несовместимые биты
                    uint uncomp = groupdata[i] ^ (v.data[i] & groupmask[i]);
                    // убираем маску по несовместимым битам
                    groupmask[i] &= ~uncomp;
                    groupdata[i] &= groupmask[i];
                }
                if (groupmask[i] > 0) gs++;
            }
            
            var group = new Vector(store, -1, groupdata, groupmask);

            return (group, gs);
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < mask.Count; i++)
            {
                // проверяем соответствие битов, установленных в обоих векторах
                uint groupmask = mask[i] & v.mask[i];
                if (groupmask == 0) continue;
                if ((data[i] & groupmask) != (v.data[i] & groupmask)) return false;
            }
            return true;
        }

        public bool ExtendTo(Vector v)
        {
            bool changed = false;
            for (int i = 0; i < mask.Count; i++)
            {
                uint d = data[i];
                uint m = mask[i];
                uint groupmask = ~mask[i] & v.mask[i];
                data[i] |= v.data[i] & groupmask;
                mask[i] |= groupmask;
                changed |= d != data[i] || m != mask[i];
            }
            return changed;
        }

        public bool Removed => store.VectorRemoved(index);
        public void Remove() => store.RemoveVector(index);

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(store.VariablesCount);
            for (int i = 0; i < store.VariablesCount; i++)
            {
                int shift = i % 32;
                var index = (i - shift) / 32;
                sb.Append(((mask[index] >> shift) & 1) == 0 ? "x" : ((data[index] >> shift) & 1) == 1 ? "1" : "0");
            }
            return sb.ToString();
        }
    }
}
