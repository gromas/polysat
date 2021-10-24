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
                    bool removed = false;

                    var vectors = c.GetVectors().ToArray();
                    if (vectors.Length == 0) return false;

                    foreach (var vector in vectors)
                    {
                        foreach (var cc in store.GetCombinations())
                        {
                            var compatible = cc.GetCompatible(vector).ToArray();

                            switch (compatible.Length)
                            {
                                case 0:
                                    if (vectors.Length == 1) return false;
                                    c.Remove(vector);
                                    changed = true;
                                    removed = true;
                                    break;
                                case 1:
                                    var ext = vector.ExtendTo(compatible[0]);
                                    changed |= ext;
                                    break;
                                default:
                                    var (group, gc) = vector.Group(compatible);
                                    if (gc != 0)
                                    {
                                        var extg = vector.ExtendTo(group);
                                        changed |= extg;
                                    }
                                    break;
                            }
                            if (removed) break;
                        }
                        if (removed) break;
                    }
                }
            } 
            while (changed);

            return true;
        }
    }
}
