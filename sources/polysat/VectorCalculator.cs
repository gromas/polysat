using PolySat.Solver;
using System;
using System.Collections.Generic;
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
            var kernel = new Kernel(store);
            bool changed;

            do
            {
                changed = false;

                Console.Write($"{DateTime.Now} ALL ");
                if (!IsSatisfable(kernel, kernel.GetCombinations().ToHashSet()))
                {
                    return false;
                }

                foreach (var c in kernel.GetCombinations())
                {
                    var vectors = kernel.GetVectors(c).ToArray();
                    if (vectors.Length == 0) return false;

                    HashSet<(Vector, VectorSnapshot)> vectorSnapshot = new();

                    var snapshot = kernel.Snapshot();
                    byte removed = 0;

                    foreach (var vector in vectors)
                    {
                        kernel.RemoveExcept(vector);
                        Console.Write($"{DateTime.Now} ({c.Index}:{vector.vIndex}) ");
                        if (!IsSatisfable(kernel, new[] { c }.ToHashSet()))
                        {
                            removed |= (byte)(1 << vector.vIndex);
                        }
                        else
                        {
                            vectorSnapshot.Add((vector, vector.Snapshot()));
                        }
                        kernel.Restore(snapshot);
                    }
                    foreach (var (v, s) in vectorSnapshot)
                    {
                        changed |= v.Restore(s);
                    }
                    if (!kernel.Remove(c, removed)) continue;
                    Console.Write($"{DateTime.Now} ({c.Index}  ) ");
                    if (!IsSatisfable(kernel, new[] { c }.ToHashSet()))
                    {
                        return false;
                    }
                    changed = true;
                }

            } while (changed);

            return true;
        }

        private static bool IsSatisfable(Kernel kernel, HashSet<Combination> changes)
        {
            do
            {
                Console.Write($"{changes.Count} ");

                var staged = changes;
                changes = new();

                foreach (var c in kernel.GetCombinations())
                {
                    bool changed = false;
                    var vectors = kernel.GetVectors(c).ToArray();
                    int vl = vectors.Length;
                    if (vl == 0)
                    {
                        Console.WriteLine(" UNSAT");
                        return false;
                    }

                    foreach (var vector in vectors)
                    {
                        foreach (var cc in staged)
                        {
                            if (vector.c.Equals(cc)) continue;

                            var compatible = vector.GetCompatible(kernel.GetVectors(cc)).ToArray();

                            switch (compatible.Length)
                            {
                                case 0:
                                    if (--vl == 0)
                                    {
                                        Console.WriteLine(" UNSAT");
                                        return false;
                                    }
                                    kernel.Remove(vector);
                                    changed = true;
                                    goto removed;
                                default:
                                    (Vector group, int gc) = vector.Group(compatible);
                                    changed |= gc != 0 && vector.Apply(group);
                                    break;
                            }
                        }
                    removed: { }
                    }
                    if (changed) changes.Add(c);
                }
            } while (changes.Count > 0);

            Console.WriteLine(" SAT");
            return true;
        }
    }
}
