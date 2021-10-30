using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat.Solver
{
    internal struct VectorSnapshot
    {
        public readonly ulong[] data;
        public readonly ulong[] mask;

        public VectorSnapshot(ArraySegment<ulong> data, ArraySegment<ulong> mask)
        {
            this.data = data.ToArray();
            this.mask = mask.ToArray();
        }
    }
}
