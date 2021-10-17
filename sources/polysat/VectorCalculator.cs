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

                foreach(var c in store.Combinations)
                {
                    bool removed = false;

                    var vectors = c.GetVectors().ToArray();
                    if (vectors.Length == 0) return false;

                    foreach (var vector in vectors)
                    {
                        foreach (var cc in store.Combinations)
                        {
                            if (cc.Equals(c)) continue;

                            var compatible = cc.GetCompatible(vector).ToArray();

                            switch (compatible.Length)
                            {
                                case 0:
                                    if (vectors.Length == 1) return false;
                                    vector.IsRemoved = true;
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
            } while (changed);

            return true;
        }
    }
}
