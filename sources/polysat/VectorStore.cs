using System.Collections.Generic;
using System.Diagnostics;

namespace PolySat
{
    public class VectorStore
    {
        private readonly int n;
        private readonly Dictionary<long, Combination> combinations;
        public VectorStore(int n)
        {
            this.n = n;
            combinations = new Dictionary<long, Combination>();
        }

        public void AddConstraint((int x0, int x1, int x2) constraint)
        {
            var x = Utils.SortByAbs(constraint.x0, constraint.x1, constraint.x2);
            var cindex = GetIndex((x[0][0], x[1][0], x[2][0]));
            if (!combinations.TryGetValue(cindex, out Combination c))
            {
                c = new Combination(n, (x[0][0], x[1][0], x[2][0]));
                combinations.Add(cindex, c);
            }
            byte vindex = (byte)((x[2][1] > 0 ? 0 : 1) + (x[1][1] > 0 ? 0 : 2) + (x[0][1] > 0 ? 0 : 4));
            c.Remove(vindex);
        }

        private long GetIndex((int x0, int x1, int x2) x)
        {
            Debug.Assert(x.x0 < x.x1 && x.x1 < x.x2 && x.x2 <= n, "Index out of range");

            long s0 = x.x2 - x.x1 - 1;
            long s1 = (1 + x.x0 - x.x1) * (x.x0 + x.x1 - 2 * n) / 2;
            long s2 = (x.x0 - 1) * (3 * n * n - 3 * n * x.x0 - 3 * n + x.x0 * x.x0 + x.x0) / 6;

            return s0 + s1 + s2;
        }

        public IEnumerable<Combination> GetCombinations()
        {
            return combinations.Values;
        }

        public void AddConstraints(IEnumerable<int[]> constraints)
        {
            foreach (var constraint in constraints)
            {
                // TODO: add k-CNF constraints
                AddConstraint((constraint[0], constraint[1], constraint[2]));
            }
        }
    }
}
