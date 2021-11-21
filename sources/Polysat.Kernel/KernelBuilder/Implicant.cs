using System;
using System.Collections.Generic;
using System.Linq;

namespace PolySat.KernelBuilder
{
    public sealed class Implicant : HashSet<int>, IEquatable<Implicant>
    {
        public Implicant(IEnumerable<int> v) => UnionWith(v);
        public bool Equals(Implicant other) => this.SequenceEqual(other);
        public override int GetHashCode() => Count.GetHashCode();
        public override bool Equals(object obj) => obj is Implicant impl && Equals(impl);
        public override string ToString() => string.Join(", ", this.OrderBy(x => Math.Abs(x)));
    }
}
