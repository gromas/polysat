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
                var temp = mask.ToArray();

                for (int x0 = 0; x0 < n; x0++)
                {
                    if (temp[x0] == 0) continue;

                    for (int x1 = 0; x1 < n; x1++)
                    {
                        if (x0 == x1) continue;

                        for (int x2 = 0; x2 < n; x2++)
                        {
                            if (temp[x2] != 0 || x2 == x0 || x2 == x1) continue;
                            yield return new Combination(x0 + 1, x1 + 1, x2 + 1);
                        }
                    }
                }
            }
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
