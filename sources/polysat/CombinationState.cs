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
            new byte[] {0,0,0 },
            new byte[] {0,0,1 },
            new byte[] {0,1,0 },
            new byte[] {0,1,1 },
            new byte[] {1,0,0 },
            new byte[] {1,0,1 },
            new byte[] {1,1,0 },
            new byte[] {1,1,1 },
        };

        private readonly byte[] state;
        private readonly Combination c;
        public CombinationState(Combination c, int state)
        {
            this.state = states[state];
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
                if (varnum == c[0]) return state[0];
                if (varnum == c[1]) return state[1];
                if (varnum == c[2]) return state[2];

                throw new IndexOutOfRangeException();
            }
        }
    }
}
