using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PolySat
{
    public class VectorStore
    {
        private int version = 0;
        private readonly Stack<IRemovable> removed;
        private readonly int n;
        private readonly int vectorSize;
        private readonly Dictionary<(int x0, int x1, int x2), Combination> combinations;

        public VectorStore(int n)
        {
            this.n = n;
            removed = new Stack<IRemovable>();
            vectorSize = (n - 1 - (n - 1) % 64) / 64 + 1;
            combinations = new Dictionary<(int x0, int x1, int x2), Combination>();
        }

        public Combination GetCombination((int x0, int x1, int x2) index)
        {
            return combinations[index];
        }

        private VectorStore(int n, int vectorSize, Dictionary<(int x0, int x1, int x2), Combination> combinations)
        {
            this.n = n;
            this.vectorSize = vectorSize;
            this.combinations = combinations.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Snapshot(this));
        }

        public void AddConstraint((int x0, int x1, int x2) constraint)
        {
            var x = Utils.SortByAbs(constraint.x0, constraint.x1, constraint.x2);
            if (!combinations.TryGetValue((x[0][0], x[1][0], x[2][0]), out Combination c))
            {
                c = new Combination(this, (x[0][0], x[1][0], x[2][0]));
                combinations.Add((x[0][0], x[1][0], x[2][0]), c);
            }
            byte vindex = (byte)((x[2][1] > 0 ? 0 : 1) + (x[1][1] > 0 ? 0 : 2) + (x[0][1] > 0 ? 0 : 4));
            c.Remove(vindex);
        }

        public void AddConstraints(IEnumerable<int[]> constraints)
        {
            foreach (var constraint in constraints)
            {
                // TODO: add k-CNF constraints
                AddConstraint((constraint[0], constraint[1], constraint[2]));
            }
        }

        public void Save(IRemovable r)
        {
            removed.Push(r);
            version++;
        }

        public void Restore(int version)
        {
            for (; this.version > version; this.version--)
            {
                var r = removed.Pop();
                r.Restore();
            }
        }

        public void Cleanup()
        {
            // clean vectors data
            foreach (var v in Combinations.SelectMany(c => c.Vectors))
            {
                v.Cleanup();
            }
        }

        public int VariableCount => n;
        public int Version => version;
        public int VectorSize => vectorSize;
        public IEnumerable<Combination> Combinations => combinations.Values.Where(c => !c.IsRemoved);

        public VectorStore Snapshot()
        {
            return new VectorStore(n, vectorSize, combinations);
        }
    }
}
