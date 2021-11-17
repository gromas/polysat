using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat.Solver
{
    internal struct VectorSnapshot
    {
        public readonly ulong[] bitSet0;
        public readonly ulong[] bitSet1;

        public VectorSnapshot(ArraySegment<ulong> bitSet0, ArraySegment<ulong> bitSet1)
        {
            this.bitSet0 = bitSet0.ToArray();
            this.bitSet1 = bitSet1.ToArray();
        }
    }
}
