using System.Collections.Generic;

namespace PolySat
{
    public class VectorStore
    {
        internal readonly int n;
        internal readonly IDictionary<Combination, byte> functions;
        public VectorStore(int n)
        {
            this.n = n;
            functions = new Dictionary<Combination, byte>();
        }

        public void AddConstraint((int x0, int x1, int x2) constraint)
        {
            var x = Utils.SortByAbs(constraint.x0, constraint.x1, constraint.x2);
            byte vIndex = (byte)((x[2][1] > 0 ? 0 : 1) + (x[1][1] > 0 ? 0 : 2) + (x[0][1] > 0 ? 0 : 4));
            Combination combination = new Combination((x[0][0], x[1][0], x[2][0]));
            if (!functions.ContainsKey(combination))
            {
                functions.Add(combination, 0x00);
            }
            functions[combination] |= (byte)(1 << vIndex);
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
