using System.Collections.Generic;

namespace PolySat
{
    /// <summary>
    /// Combination: represents an combination 3 of n
    /// </summary>
    public class Combination
    {
        private readonly VectorStore store;
        private readonly int index;

        public Combination(VectorStore store, int index)
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
            for (int i = 0; i < 8; i++)
            {
                if (!store.VectorRemoved(index * 8 + i))
                {
                    yield return store[index * 8 + i];
                }
            }
        }

        public IEnumerable<Vector> GetCompatible(Vector s)
        {
            for (int i = 0; i < 8; i++)
            {
                if (!store.VectorRemoved(index * 8 + i))
                {
                    Vector v = store[index * 8 + i];
                    if (s.IsCompatible(v)) yield return v;
                }
            }
        }

        public int Index => index;
    }
}
