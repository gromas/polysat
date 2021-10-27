using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    // гипотеза: условием независимости переменной от значений двух других
    // переменных сочетания считаем наличие совпадающей пары назначений x1 и x2
    // для назначений x0 = 1 и x0 = 0 одновременно. Соответственно, если существует
    // назначение, противоречащее нашему утверждению - переменная возможно не является фиктивной
    // TODO: проверить влияние других назначений переменных в векторах на правильность утверждения

    public class DummyVariablesExplorer
    {
        private readonly VectorStore store;
        private readonly HashSet<int> dummyVars;

        public bool Unsatisfable => store.VariableCount == dummyVars.Count;

        public DummyVariablesExplorer(VectorStore store)
        {
            this.store = store;
            dummyVars = new HashSet<int>();
        }

        public IEnumerable<int> GetPureDummyVariablesAndCollapse()
        {
            return GetPureDummyVariablesAndCollapse(dummyVars);
        }

        // утверждение: условием глобальной фиктивности переменной является её локальная независимость от всех пар переменных
        private IEnumerable<int> GetPureDummyVariablesAndCollapse(HashSet<int> dummyVars)
        {
            for (int x = 1; x <= store.VariableCount; x++)
            {
                if (dummyVars.Contains(x)) continue;

                bool isDummy = true;

                var combinations = store.Combinations.Where(c => c.Index.x0 == x || c.Index.x1 == x || c.Index.x2 == x).ToArray();
                // если среди сочетаний существует хотя бы одна существенная пара переменных,
                // то исследуемая переменная не может быть глобально фиктивной
                foreach (var c in combinations)
                {
                    var xx = new int[] { c.Index.x0, c.Index.x1, c.Index.x2 }.Except(new int[] { x }).ToArray();
                    if (c.Vectors
                        .GroupBy(g => (g.GetBit(xx[0]), g.GetBit(xx[1])))
                        .Where(g => g.Count() != 2).Any())
                    {
                        isDummy = false;
                        break;
                    }
                }
                if (isDummy)
                {
                    //// схлопнем фиктивные векторы
                    CollapseDummyVariableVectors(x);
                    dummyVars.Add(x);
                    yield return x;
                }
            }
        }

        // если переменная является глобально фиктивной, устанавливаем её значение во всех сочетаниях в 0
        private void CollapseDummyVariableVectors(int x)
        {
            var combinations = store.Combinations.Where(c => c.Index.x0 == x || c.Index.x1 == x || c.Index.x2 == x).ToArray();
            foreach (var c in combinations)
            {
                var xx = new int[] { c.Index.x0, c.Index.x1, c.Index.x2 }.Except(new int[] { x }).ToArray();
                var tbd = c.Vectors
                    .GroupBy(g => (g.GetBit(xx[0]), g.GetBit(xx[1])))
                    .Where(g => g.Count() == 2)
                    .SelectMany(g => g.Where(v => v.GetBit(x) == 1));
                foreach (var v in tbd)
                {
                    v.Remove();
                }
            }
        }

        public bool DeepSearch()
        {
            var calc = new VectorCalculator(store);
            bool changed;

            do
            {
                changed = false;

                for (int x = 1; x <= store.VariableCount; x++)
                {
                    if (dummyVars.Contains(x)) continue;

                    var combinations = store.Combinations.Where(c => c.Index.x0 == x || c.Index.x1 == x || c.Index.x2 == x).ToArray();

                    // проверим каждый вектор ess на выполнимость
                    foreach (var c in combinations)
                    {
                        // Приведу в соответствие с последним видео
                        var vectors = c.Vectors.ToArray();

                        foreach (var v in vectors)
                        {
                            var version = store.Version;
                            foreach (var tbd in vectors)
                            {
                                if (tbd.Index == v.Index) continue;
                                tbd.Remove();
                            }
                            var sat = calc.IsSatisfable();
                            store.Restore(version);
                            if (!sat)
                            {
                                changed = true;
                                v.Remove();
                            }
                        }
                    }

                    foreach(var d in GetPureDummyVariablesAndCollapse())
                    {
                        Console.WriteLine($"var {d} evaluated to dummy variable");
                        dummyVars.Add(d);
                        if (Unsatisfable) return false;
                    }
                }
            } while (changed);
            
            return calc.IsSatisfable();
        }

        private (Vector[] dummies, Vector[] essentials) Split(Combination c, int x)
        {
            var xx = new int[] { c.Index.x0, c.Index.x1, c.Index.x2 }.Except(new int[] { x }).ToArray();
            var dummies = c.Vectors
                        .GroupBy(g => (g.GetBit(xx[0]), g.GetBit(xx[1])))
                        .Where(g => g.Count() == 2).SelectMany(v => v);
            var essentials = c.Vectors
                        .GroupBy(g => (g.GetBit(xx[0]), g.GetBit(xx[1])))
                        .Where(g => g.Count() != 2).SelectMany(v => v);
            return (dummies.ToArray(), essentials.ToArray());
        }

    }
}
