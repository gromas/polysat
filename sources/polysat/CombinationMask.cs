using System;
using System.Collections.Generic;
using System.Linq;

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
            int cs = n-1, cu = 0;

            for(int i = 0; i < n; i++)
            {
                if (mask[i] == 0)
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

        public void ApplyState(CombinationState state)
        {
            var c = state.Combination;
            this[c[0]] = state[c[0]];
            this[c[1]] = state[c[1]];
            this[c[2]] = state[c[2]];
        }

        public byte this[int index]
        {
            get
            {
                return (byte)(mask[index - 1] ^ NotSet);
            }
            set
            {
                mask[index - 1] = (byte)(value ^ NotSet);
            }
        }
    }
}
