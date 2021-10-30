using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat.Solver
{
    internal struct KernelSnapshot
    {
        public readonly ulong[] data;
        public readonly byte[] removed;

        public KernelSnapshot(ArraySegment<ulong> data, ArraySegment<byte> removed)
        {
            this.data = data.ToArray();
            this.removed = removed.ToArray();
        }
    }
}
