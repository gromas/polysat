using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PolySat
{
    /// <summary>
    /// Combination full vector class
    /// </summary>
    public class Vector
    {
        const int NotSet = 2;
        private readonly VectorStore store; 
        private readonly uint index;
        private readonly ArraySegment<byte> state;
        private Vector(VectorStore store, ArraySegment<byte> state)
        {
            this.store = store;
            this.state = state;
        }
        public Vector(VectorStore store, uint index) : 
            this(store, new ArraySegment<byte>(store.Bytes, (int)(index * store.VectorSize), (int)store.VectorSize))
        {
            this.index = index;
        }
        public Vector(VectorStore store) : this(store, new byte[store.VectorSize])
        {
        }

        public bool IsRemoved
        {
            get
            {
                return store.IsRemoved(index);
            }
            set
            {
                if (!value) throw new Exception("can't restore");
                store.Remove(index);
            }
        }

        private int GetValue(uint x)
        {
            x -= 1;
            var b = (x - x % 4) / 4;
            var bp = (3 - x % 4) * 2;
            return (state[(int)b] >> (int)bp) & 3;
        }

        /// <summary>
        /// Set vector variable value
        /// </summary>
        /// <param name="state">vector state</param>
        /// <param name="x">variable index</param>
        /// <param name="v">value</param>
        public void SetValue(uint x, int v)
        {
            x -= 1;
            uint b = (x - x % 4) / 4;
            uint bp = (3 - x % 4) * 2;

            int s0 = 3 << (int)bp;
            int s1 = (state[(int)b] ^ 0xFF) | s0;
            int s2 = (s1 & ((v << (int)bp) ^ 0xFF)) ^ 0xFF;

            state[(int)b] = (byte)s2;
        }

        public bool ExtendTo(Vector v)
        {
            bool changed = false;
            for (uint i = 1; i <= store.VariablesCount; i++)
            {
                var thisValue = GetValue(i);
                if (thisValue != NotSet) continue;

                var otherValue = v.GetValue(i);
                if (otherValue == NotSet) continue;

                SetValue(i, otherValue);
                changed = true;
            }
            return changed;
        }

        public (Vector, int) Group(Vector[] compatible)
        {
            uint gs = store.VariablesCount;
            var group = new Vector(store);

            for (uint i = 1; i <= store.VariablesCount; i++)
            {
                group.SetValue(i, NotSet);
                int gvv = NotSet;
                foreach (var v in compatible)
                {
                    int vv = v.GetValue(i);
                    if (vv == NotSet)
                    {
                        group.SetValue(i, NotSet);
                        gs--;
                        break;
                    }
                    if (gvv != NotSet && gvv != vv)
                    {
                        group.SetValue(i, NotSet);
                        gs--;
                        break;
                    }
                    if (gvv == NotSet)
                    {
                        group.SetValue(i, vv);
                        gvv = vv;
                    }
                }
                int cv = GetValue(i);
                if (cv != NotSet && cv == group.GetValue(i))
                {
                    gs--;
                }
            }
            return (group, (int)gs);
        }

        public bool IsCompatible(Vector v)
        {
            bool compatible = true;
            for (uint i = 1; i <= store.VariablesCount; i++)
            {
                var thisValue = GetValue(i);
                // notset compatible with any other
                if (thisValue == NotSet) continue;
                
                var otherValue = v.GetValue(i);
                if (otherValue == NotSet) continue;

                if (thisValue != otherValue)
                {
                    compatible = false;
                    break;
                }
            }
            return compatible;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder((int)store.VariablesCount);
            for (int i = 0; i < store.VariablesCount; i++)
            {
                var b = (i - i % 4) / 4;
                var s = (3 - i % 4) * 2;
                sb.Append(((state[b] >> s) & 3) == NotSet ? "x" : ((state[b] >> s) & 3) == 1 ? "1" : "0");
            }
            if (IsRemoved) sb.Append("--");
            return sb.ToString();
        }

        public Vector Clone()
        {
            return new Vector(store, state.ToArray());
        }

        public IEnumerable<Vector> GetCompatible(IEnumerable<Vector> s)
        {
            return s.Where(v => IsCompatible(v));
        }
    }
}
