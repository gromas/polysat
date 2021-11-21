using System;
using System.Collections.Generic;
using System.Linq;

namespace PolySat.KernelBuilder
{
    public sealed class Constituent : HashSet<int>
    {
        public Constituent(IEnumerable<int> v) => UnionWith(v);
        public override string ToString() => string.Join(", ", this.OrderBy(x => Math.Abs(x)));
    }
}
