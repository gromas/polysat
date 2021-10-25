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

        /// <summary>
        /// Fictional variable test
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public bool IsFictional(int x)
        {
            // гипотеза: условием независимости переменной от значений двух других
            // переменных сочетания считаем наличие совпадающей пары назначений x1 и x2
            // для назначений x0 = 1 и x0 = 0 одновременно. Соответственно, если существует
            // назначение, противоречащее нашему утверждению - переменная возможно не является фиктивной
            // TODO: проверить влияние других назначений переменных в векторах на правильность утверждения

            bool fictional = true;

            Vector mask1 = new Vector(store.VariableCount, x, 1);
            Vector mask0 = new Vector(store.VariableCount, x, 0);

            foreach (var c in store.GetCombinations().Where(c=>c.Index.x0 == x || c.Index.x1 == x || c.Index.x2 == x))
            {
                var xx = new int[] { c.Index.x0, c.Index.x1, c.Index.x2 }.Except(new int[] { x }).ToArray();
                fictional &= !c.GetVectors()
                    .GroupBy(g => (g.GetBit(xx[0]), g.GetBit(xx[1])))
                    .Where(g => g.Count() != 2).Any();
            }
            return fictional;
        }
    }
}
