using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                onRemoved:
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
                                    store.AddConstraint(
                                        s.GetValue(c[0]) > 0 ? c[0] : -c[0],
                                        s.GetValue(c[1]) > 0 ? c[1] : -c[1],
                                        s.GetValue(c[2]) > 0 ? c[2] : -c[2]);
                                    //s.IsRemoved = true;
                                    changed = true;
                                    goto onRemoved;
                                case 1:
                                    var ext = s.ExtendTo(compatible[0]);
                                    changed |= ext;
                                    break;
                                case 2:
                                    break;
                                default:
                                    break;
                            }
                            //Console.WriteLine(s);
                            //changed = true;
                        }
                    }
                }
            } while (changed);

            // check removed

            foreach (var c in store.Combinations)
            {
                foreach(var s in store.GetRemoved(c))
                {
                    w.WriteLine($"{s}");

                    foreach (var cc in store.Combinations)
                    {
                        if (cc.Equals(c)) continue;

                        var compatible = store.GetCompatible(cc, s).ToArray();

                        switch (compatible.Length)
                        {
                            case 0:
                                break;
                            default:
                                foreach(var cs in compatible)
                                {
                                    w.WriteLine($"{cs}");
                                }
                                break;
                        }
                    }
                }
            }


            return true;
        }
    }
}
