using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace PolySat
{
    /// <summary>
    /// Represents combination state calculation mask
    /// </summary>
    public class CombinationMask
    {
        const byte NotSet = 2;
        /// <summary>
        /// Calculation mask - one byte for each variable
        /// 0x0 - variable set to Zero
        /// 0x1 - variable set to One
        /// 0x2 - variable ambigious or unset
        /// </summary>
        private readonly byte[] mask;
        private readonly int n;

        public CombinationMask(int n)
        {
            this.n = n;
            mask = new byte[n];
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < n; i++)
            {
                mask[i] = NotSet;
            }
        }

        /// <summary>
        /// Returns all combinations has least one unset variable and one set variable
        /// </summary>
        public IEnumerable<Combination> UnresolvedCombinations
        {
            get
            {
                var (temp, p) = SplitMask();

                for (int x0 = 0; x0 < p; x0++)
                {
                    for (int x1 = p; x1 < n; x1++)
                    {
                        for (int x2 = x0 + 1; x2 < x1; x2++)
                        {
                            yield return new Combination(temp[x0] + 1, temp[x1] + 1, temp[x2] + 1);
                        }
                    }
                }
            }
        }

        private (int[], int) SplitMask()
        {
            int[] r = new int[n];
            int cs = n - 1, cu = 0;

            for (int i = 0; i < n; i++)
            {
                if (mask[i] == NotSet)
                {
                    r[cu] = i;
                    cu++;
                }
                else
                {
                    r[cs] = i;
                    cs--;
                }
            }
            return (r, cu);
        }

        // Apply single state to mask
        public bool ApplyState(CombinationState s)
        {
            bool changed = false;
            var c = s.Combination;
            for (int i = 0; i < 3; i++)
            {
                if (this[c[i]] == NotSet)
                {
                    // single state can change two bits
                    changed = true;
                    this[c[i]] = s[c[i]];
                }
            }
            return changed;
        }

        public bool ApplyStates(CombinationState s0, CombinationState s1)
        {
            Debug.Assert(s0.Combination.Equals(s1.Combination), "Can't apply states of two different combinations");
            var c = s0.Combination;
            for (int i = 0; i < 3; i++)
            {
                var s = s0[c[i]];
                if (mask[c[i] - 1] == NotSet && s == s1[c[i]])
                {
                    mask[c[i] - 1] = s;
                    // only one bit can be set from two compatible states
                    return true;
                }
            }
            return false;
        }

        public byte this[int index]
        {
            get
            {
                return mask[index - 1];
            }
            set
            {
                mask[index - 1] = value;
            }
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder(mask.Length);
            for (int i = 0; i < mask.Length; i++) b.Append(mask[i] == 2 ? "x" : mask[i] == 1 ? "1" : "0");
            return b.ToString();
        }

        public int Size
        {
            get
            {
                var (_, size) = SplitMask();
                return size;
            }
        }
    }
}
