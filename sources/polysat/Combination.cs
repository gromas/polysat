using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    public class Combination
    {
        private readonly (int x0, int x1, int x2) index;
        private readonly Dictionary<byte, Vector> vectors;

        public Combination(int n, (int x0, int x1, int x2) index)
        {
            this.index = index;
            vectors = new Dictionary<byte, Vector>();
            for (byte i = 0; i < 8; i++)
            {
                vectors.Add(i, new Vector(n, (index.x0, index.x1, index.x2, i)));
            }
        }

        public IEnumerable<Vector> GetVectors()
        {
            return vectors.Values;
        }

        public void Remove(Vector vector)
        {
            vectors.Remove(vector.Index.vindex);
        }

        public void Remove(byte vindex)
        {
            vectors.Remove(vindex);
        }

        public IEnumerable<Vector> GetCompatible(Vector vector)
        {
            return vectors.Values.Where(v => vector.IsCompatible(v));
        }

        public bool IsVectorRemoved(byte index)
        {
            return !vectors.ContainsKey(index);
        }

        public (int x0, int x1, int x2) Index => index;
    }
}
