using System.IO;
using System.Linq;

namespace PolySat
{
    public class VectorCalculator
    {
        private readonly StreamWriter w;
        private readonly VectorStore store;
        public VectorCalculator(VectorStore store, StreamWriter w)
        {
            this.w = w;
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

                    var cv = store.GetVectors(c).ToArray();
                    if (cv.Length == 0)
                    {
                        return false;
                    }
                    foreach (var s in cv)
                    {
                        foreach (var cc in store.Combinations)
                        {
                            if (cc.Equals(c)) continue;

                            var compatible = store.GetCompatible(cc, s).ToArray();

                            switch (compatible.Length)
                            {
                                case 0:
                                    s.IsRemoved = true;
                                    changed = true;
                                    removed = true;
                                    break;
                                case 1:
                                    var ext = s.ExtendTo(compatible[0]);
                                    changed |= ext;
                                    break;
                                default:
                                    var (group, gc) = s.Group(compatible);
                                    if (gc != 0)
                                    {
                                        var extg = s.ExtendTo(group);
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
