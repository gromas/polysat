using System;

namespace PolySat
{
    /// <summary>
    /// Represents posible combination state
    /// </summary>
    public class CombinationState
    {
        /// <summary>
        /// All posible states for combination 3 of n
        /// </summary>
        private static readonly byte[][] states = new byte[8][]
        {
            new byte[] {0,0,0},
            new byte[] {0,0,1},
            new byte[] {0,1,0},
            new byte[] {0,1,1},
            new byte[] {1,0,0},
            new byte[] {1,0,1},
            new byte[] {1,1,0},
            new byte[] {1,1,1},
        };

        private int state;
        private readonly byte[] mask;
        private readonly Combination c;

        public CombinationState(Combination c, int state)
        {
            this.state = state;
            mask = states[state];
            this.c = c;
        }

        /// <summary>
        /// Returns state value by variable number
        /// </summary>
        /// <param name="varnum"></param>
        /// <returns></returns>
        public byte this[int varnum]
        {
            get
            {
                if (varnum == c[0]) return mask[0];
                if (varnum == c[1]) return mask[1];
                if (varnum == c[2]) return mask[2];

                throw new IndexOutOfRangeException();
            }
        }

        public Combination Combination => c;
        public int State => state;

        public override string ToString()
        {
            return $"{(mask[0] == 0 ? c[0] :-c[0])} {(mask[1] == 0 ? c[1] : -c[1])} {(mask[2] == 0 ? c[2] : -c[2])} 0";
        }
    }
}
