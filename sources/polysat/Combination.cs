using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    public class Combination : IRemovable
    {
        private bool removed;
        private readonly VectorStore store;
        private readonly (int x0, int x1, int x2) index;
        private readonly Dictionary<byte, Vector> vectors;

        public Combination(VectorStore store, (int x0, int x1, int x2) index)
        {
            this.store = store;
            this.index = index;
            vectors = new Dictionary<byte, Vector>();
            for (byte i = 0; i < 8; i++)
            {
                vectors.Add(i, new Vector(store, (index.x0, index.x1, index.x2, i)));
            }
        }

        public IEnumerable<Vector> Vectors => vectors.Values.Where(v => !v.IsRemoved);

        public void Remove(byte vindex)
        {
            vectors[vindex].Remove();
        }

        public void Remove()
        {
            removed = true;
            store.Save(this);
        }

        public IEnumerable<Vector> GetCompatible(Vector vector)
        {
            return Vectors.Where(v => vector.IsCompatible(v));
        }

        public (int x0, int x1, int x2) Index => index;
        /// <summary>
        /// Combination removed is current version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public bool IsRemoved => removed;

        void IRemovable.Restore()
        {
            removed = false;
        }
    }
}
