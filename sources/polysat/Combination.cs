using System;
using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    /// <summary>
    /// Combination: represents an combination 3 of n
    /// </summary>
    public class Combination
    {
        private readonly VectorStore store;
        private readonly uint index;

        public Combination(VectorStore store, uint index)
        {
            this.store = store;
            this.index = index;
        }

        public bool Equals(Combination other)
        {
            return index == other.index;
        }

        public IEnumerable<Vector> GetVectors()
        {
            for (uint i = 0; i < 8; i++)
            {
                var v = new Vector(store, index * 8 + i);
                if (!v.IsRemoved) yield return v;
            }
        }

        public IEnumerable<Vector> GetCompatible(Vector s)
        {
            var vv = GetVectors();
            foreach (var v in vv)
            {
                if (v.IsRemoved) continue;
                if (s.IsCompatible(v))
                {
                    yield return v;
                }
            }
        }

        public uint Index => index;

        /// TODO:
        //public override string ToString()
        //{
        //    return $"{{{x[0]},{x[1]},{x[2]}}}";
        //}
    }
}
