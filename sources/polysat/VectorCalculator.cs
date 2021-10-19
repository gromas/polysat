using System.IO;
using System.Linq;

namespace PolySat
{
    public class VectorCalculator
    {
        private readonly VectorStore store;
        private readonly StreamWriter w;
        public VectorCalculator(VectorStore store, StreamWriter w)
        {
            this.store = store;
            this.w = w;
        }

        public bool IsSatisfable()
        {
            w.WriteLine("vector calculate STARTED");
            bool changed;
            do
            {
                changed = false;

                foreach (var c in store.Combinations())
                {
                    bool removed = false;

                    var vectors = c.GetVectors().ToArray();
                    if (vectors.Length == 0) return false;

                    foreach (var vector in vectors)
                    {
                        foreach (var cc in store.Combinations(/*vector*/))
                        {
                            w.WriteLine();
                            w.WriteLine($"check {vector} in combination {cc.Index}");

                            var compatible = cc.GetCompatible(vector).ToArray();



                            switch (compatible.Length)
                            {
                                case 0:
                                    w.WriteLine("combination vectors:");
                                    foreach(var v in cc.GetVectors())
                                    {
                                        w.WriteLine($"{v}");
                                    }
                                    w.WriteLine($"no compatible vectors found");
                                    w.WriteLine($"vector {vector} removed.");
                                    if (vectors.Length == 1)
                                    {
                                        w.WriteLine($"no more vectors in combination {c.Index}.");
                                        return false;
                                    }
                                    vector.Remove();
                                    changed = true;
                                    removed = true;
                                    break;
                                case 1:
                                    w.WriteLine($"found 1 compatible vector {compatible[0]}.");
                                    var ext = vector.ExtendTo(compatible[0]);
                                    changed |= ext;
                                    if (ext)
                                    {
                                        w.WriteLine($"assigments redefined to {vector}");
                                    }
                                    else
                                    {
                                        w.WriteLine($"{vector} assigments not redefined");
                                    }
                                    break;
                                default:
                                    w.WriteLine($"found {compatible.Length} compatible vectors:");
                                    foreach(var v in compatible)
                                    {
                                        w.WriteLine(v);
                                    }
                                    var (group, gc) = vector.Group(compatible);
                                    w.WriteLine($"----------------------");
                                    w.WriteLine($"group vector {group}");
                                    if (gc != 0)
                                    {

                                        var extg = vector.ExtendTo(group);
                                        if (extg)
                                        {
                                            w.WriteLine($"assigments redefined to {vector}");
                                        }
                                        else
                                        {
                                            w.WriteLine($"{vector} assigments not redefined");
                                        }
                                        changed |= extg;
                                    }
                                    else
                                    {
                                        w.WriteLine($"{vector} assigments not redefined");
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
