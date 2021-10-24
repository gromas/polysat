using System.Collections.Generic;
using System.Diagnostics;

namespace PolySat
{
    public class VectorStore
    {
        private readonly int n;
        private readonly Dictionary<(int x0, int x1, int x2), Combination> combinations;
        public VectorStore(int n)
        {
            this.n = n;
            combinations = new Dictionary<(int x0, int x1, int x2), Combination>();
        }

        public void AddConstraint((int x0, int x1, int x2) constraint)
        {
            var x = Utils.SortByAbs(constraint.x0, constraint.x1, constraint.x2);
            if (!combinations.TryGetValue((x[0][0], x[1][0], x[2][0]), out Combination c))
            {
                c = new Combination(n, (x[0][0], x[1][0], x[2][0]));
                combinations.Add((x[0][0], x[1][0], x[2][0]), c);
            }
            byte vindex = (byte)((x[2][1] > 0 ? 0 : 1) + (x[1][1] > 0 ? 0 : 2) + (x[0][1] > 0 ? 0 : 4));
            c.Remove(vindex);
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
