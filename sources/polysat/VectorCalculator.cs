using System.Linq;

namespace PolySat
{
    public class VectorCalculator
    {
        private readonly VectorStore store;
        public VectorCalculator(VectorStore store)
        {
            this.store = store;
        }

        public bool IsSatisfable()
        {
            bool changed;
            do
            {
                changed = false;

                foreach (var c in store.GetCombinations())
                {
                    removed:

                    var vectors = c.GetVectors().ToArray();
                    if (vectors.Length == 0) return false;

                    foreach (var vector in vectors)
                    {
                        foreach (var cc in store.GetCombinations())
                        {
                            if (c.Index == cc.Index) continue;

                            var compatible = cc.GetCompatible(vector).ToArray();

                            switch (compatible.Length)
                            {
                                case 0:
                                    if (vectors.Length == 1) return false;
                                    c.Remove(vector);
                                    changed = true;
                                    goto removed;
                                default:
                                    (Vector group, int gc) = vector.Group(compatible);
                                    if (gc != 0)
                                    {
                                        changed |= vector.ExtendTo(group);
                                    }
                                    break;
                            }
                        }
                    }
                }
            } 
            while (changed);

            return true;
        }
    }
}
