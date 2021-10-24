using System;

namespace PolySat
{
    public class Vector
    {
        private readonly ArraySegment<uint> vectorData;
        private readonly ArraySegment<uint> vectorMask;
        private readonly (int x0, int x1, int x2, byte vindex) index;
        public Vector(int n, (int x0, int x1, int x2, byte vindex) index)
        {
            this.index = index;
            var vectorSize = (n - 1 - (n - 1) % 32) / 32 + 1;
            vectorData = new uint[vectorSize];
            vectorMask = new uint[vectorSize];

            SetBit(index.x0, (index.vindex >> 2) & 1);
            SetBit(index.x1, (index.vindex >> 1) & 1);
            SetBit(index.x2, index.vindex & 1);
        }

        private void SetBit(int x, int value)
        {
            int shift = (x - 1) % 32;
            var index = (x - 1 - shift) / 32;
            vectorMask[index] |= (uint)(1 << shift);
            vectorData[index] |= (uint)(value << shift);
        }

        // group vector
        private Vector(ArraySegment<uint> vectorData, ArraySegment<uint> vectorMask)
        {
            this.vectorData = vectorData;
            this.vectorMask = vectorMask;
        }

        public bool IsCompatible(Vector v)
        {
            for (int i = 0; i < vectorMask.Count; i++)
            {
                // проверяем соответствие битов, установленных в обоих векторах
                uint groupmask = vectorMask[i] & v.vectorMask[i];
                if (groupmask == 0) continue;
                if ((vectorData[i] & groupmask) != (v.vectorData[i] & groupmask)) return false;
            }
            return true;
        }

        public bool ExtendTo(Vector v)
        {
            bool changed = false;
            for (int i = 0; i < vectorMask.Count; i++)
            {
                uint d = vectorData[i];
                uint m = vectorMask[i];
                uint groupmask = ~vectorMask[i] & v.vectorMask[i];
                vectorData[i] |= v.vectorData[i] & groupmask;
                vectorMask[i] |= groupmask;
                changed |= d != vectorData[i] || m != vectorMask[i];
            }
            return changed;
        }

        public (Vector, int) Group(Vector[] vectors)
        {
            int gs = 0;
            uint[] groupmask = new uint[vectorMask.Count];
            uint[] groupdata = new uint[vectorMask.Count];
            // create group vector outside store
            for (int i = 0; i < vectorMask.Count; i++)
            {
                // считаем маску группы
                groupdata[i] = 0;
                groupmask[i] = ~vectorMask[i];
                foreach (var v in vectors)
                {
                    // считаем текущую маску группы
                    groupmask[i] &= v.vectorMask[i];
                    // сбрасываем ранее установленные биты, если маска совместимости уменьшилась
                    groupdata[i] &= groupmask[i];
                    // считаем несовместимые биты
                    uint uncomp = groupdata[i] ^ (v.vectorData[i] & groupmask[i]);
                    // убираем маску по несовместимым битам
                    groupmask[i] &= ~uncomp;
                    groupdata[i] &= groupmask[i];
                }
                if (groupmask[i] > 0) gs++;
            }

            var group = new Vector(groupdata, groupmask);

            return (group, gs);
        }

        public byte Index => index.vindex;
    }
}
