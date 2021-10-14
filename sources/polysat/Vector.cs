using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    /// <summary>
    /// Combination full vector class
    /// </summary>
    public class Vector
    {
        const int NotSet = 2;
        private readonly Combination combination;
        private readonly ArraySegment<byte> state;
        private readonly int n;
        public Vector(int n, Combination combination, ArraySegment<byte> state)
        {
            this.n = n;
            this.combination = combination;
            this.state = state;
        }

        public Combination Combination => combination;

        public bool IsRemoved
        {
            get
            {
                var b = (n - n % 4) / 4;
                var v = state[b] & 1;
                return v == 1;
            }
            set
            {
                var b = (n - n % 4) / 4;
                state[b] |= 0x01;
            }
        }

        public int GetValue(int x)
        {
            return GetValue(state, x);
        }

        private static int GetValue(ArraySegment<byte> state, int x)
        {
            x -= 1;
            var b = (x - x % 4) / 4;
            var bp = (3 - x % 4) * 2;
            return (state[b] >> bp) & 3;
        }

        private static void SetValue(ArraySegment<byte> state, int x, int v)
        {
            x -= 1;
            int b = (x - x % 4) / 4;
            int bp = (3 - x % 4) * 2;

            int s0 = 3 << bp;
            int s1 = (state[b] ^ 0xFF) | s0;
            int s2 = (s1 & ((v << bp) ^ 0xFF)) ^ 0xFF;

            state[b] = (byte)s2;
        }

        public bool ExtendTo(Vector v)
        {
            bool changed = false;
            for (int i = 1; i <= n; i++)
            {
                var thisValue = GetValue(state, i);
                if (thisValue != NotSet) continue;

                var otherValue = GetValue(v.state, i);
                if (otherValue == NotSet) continue;

                SetValue(state, i, otherValue);
                changed = true;
            }
            return changed;
        }

        public bool ExtendToGroup(Vector v1, Vector v2)
        {
            bool changed = false;
            for (int i = 1; i <= n; i++)
            {
                var thisValue = GetValue(state, i);
                if (thisValue != NotSet) continue;

                var otherValue1 = GetValue(v1.state, i);
                if (otherValue1 == NotSet) continue;

                var otherValue2 = GetValue(v2.state, i);
                if (otherValue2 == NotSet) continue;

                if (otherValue1 != otherValue2) continue;

                SetValue(state, i, otherValue1);
                changed = true;
            }
            return changed;
        }

        public (Vector, int) Group(Vector[] compatible)
        {
            int gs = n;

            byte[] group = new byte[(n - n % 4) / 4 + 1];

            for (int i = 1; i <= n; i++)
            {
                SetValue(group, i, NotSet);
                int gvv = NotSet;
                foreach (var v in compatible)
                {
                    int vv = GetValue(v.state, i);
                    if (vv == NotSet)
                    {
                        SetValue(group, i, NotSet);
                        gs--;
                        break;
                    }
                    if (gvv != NotSet && gvv != vv)
                    {
                        SetValue(group, i, NotSet);
                        gs--;
                        break;
                    }
                    if (gvv == NotSet)
                    {
                        SetValue(group, i, vv);
                        gvv = vv;
                    }
                }
                int cv = GetValue(state, i);
                if (cv != NotSet && cv == GetValue(group, i))
                {
                    gs--;
                }
            }


            return (new Vector(n, combination, group), gs);
        }

        public bool IsCompatible(Vector v)
        {
            bool compatible = true;
            for (int i = 1; i <= n; i++)
            {
                var thisValue = GetValue(state, i);
                // notset compatible with any other
                if (thisValue == NotSet) continue;
                
                var otherValue = GetValue(v.state, i);
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
            StringBuilder sb = new StringBuilder(n);
            for (int i = 0; i < n; i++)
            {
                var b = (i - i % 4) / 4;
                var s = (3 - i % 4) * 2;
                sb.Append(((state[b] >> s) & 3) == NotSet ? "x" : ((state[b] >> s) & 3) == 1 ? "1" : "0");
            }
            if (IsRemoved) sb.Append("--");
            return sb.ToString();
        }
    }
}
