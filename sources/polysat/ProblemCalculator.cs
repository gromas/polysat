using System;
using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    public class ProblemCalculator
    {
        const byte NotSet = 2;

        private readonly StateStore store;
        private CombinationMask mask;
        private CombinationMask minmask;
        private int minmaskSize = int.MaxValue;
        public ProblemCalculator(StateStore store)
        {
            this.store = store;
            minmask = mask = new CombinationMask(store.VariablesCount);
        }

        public (bool satisfable, CombinationMask minMask) IsSatisfable()
        {
            bool changed;

            /// LOOP1 - interate while each combination contains least one combination state and state changes detected
            /// Theory maximum: O(8(n^3-3n^2+2n))
            /// Practical: O(5)
            /// Each iteration removes least one combination state from existing possible states
            /// If combination states wasn't removed in current iteration then breaks the loop and return
            /// SAT if each combination contains least one combination state
            do
            {
                changed = false;
                minmaskSize = int.MaxValue;
                // LOOP2: iterate over all posible combination states of all combinations
                // Theory maximum: O(8(n^3-3n^2+2n))
                // Practical: O(8(n^3-3n^2+2n))
                // Each iteration 
                foreach (Combination c in store.GetAllCombinations())
                {
                    var states = store.GetStates(c).ToArray();
                    if (states.Length == 0) return (false, minmask);
                    // for each combination state we must check than exists consistent solution (no conflicts between mask and states)
                    foreach (CombinationState s in states)
                    {
                        // test combination state for consistency with all other combinations
                        if (!IsCompatible(s))
                        {
                            changed = true;
                            // if for current combination state not exists consisten statemask
                            // with other combinations then current combination state is a constraint
                            var remainder = store.RemoveState(s);
                            // if no more states in any combination then no solutions exists
                            if (remainder == 0) return (false, minmask);
                        }
                    }
                }
            } while (changed);

            return (true, minmask);
        }

        /// <summary>
        /// Checks current state is compatible with states of other combinations
        /// </summary>
        private bool IsCompatible(CombinationState s)
        {
            bool maskChanged;
            // create combination state mask size of n
            mask = new CombinationMask(store.VariablesCount);
            // set values for known variabless
            mask.ApplyState(s);
            // reduce mask
            // MAX: O(n^3-3n^2+2n) (if exists no more than one change for each iteration and all iterations has changes)
            do
            {
                maskChanged = false;
                // find masked conflicts : MAX O((n-3)^3)
                foreach (Combination c in mask.UnresolvedCombinations)
                {
                    var (maskUpdate, conflictDetected) = CheckStates(c);
                    if (conflictDetected) return false;

                    maskChanged |= maskUpdate();
                }
            } while (maskChanged);
            // conflicts not exist
            int maskSize = mask.Size;
            if (maskSize < minmaskSize)
            {
                minmaskSize = maskSize;
                minmask = mask;
            }
            return true;
        }

        /// <summary>
        /// Checks combination states are compatible with mask
        /// </summary>
        private (Func<bool> maskUpdate, bool conflictDetected) CheckStates(Combination c)
        {
            bool conflictDetected = false;
            Func<bool> maskUpdate = () => false;

            byte mc0 = mask[c[0]], mc1 = mask[c[1]], mc2 = mask[c[2]];

            // select compatible states (max = 8)
            CombinationState[] states = store.GetStates(c)
                .Where(s =>
                (mc0 == NotSet || mc0 == s[c[0]]) &&
                (mc1 == NotSet || mc1 == s[c[1]]) &&
                (mc2 == NotSet || mc2 == s[c[2]])).ToArray();

            switch (states.Length)
            {
                case 0:
                    // no states compatible with current mask
                    conflictDetected = true;
                    break;
                case 1:
                    // if all bits of mask are set then do nothing
                    if (mc0 != NotSet && mc1 != NotSet && mc2 != NotSet) break;
                    // single state found -> update mask and mark iteration has_changes
                    maskUpdate = () => { return mask.ApplyState(states[0]); };
                    break;
                case 2:
                    // two states found -> if both contains equals variable values then update mask
                    maskUpdate = () => { return mask.ApplyStates(states[0], states[1]); };
                    break;
                default:
                    // multiply states are compatible -> no changes
                    break;
            }
            return (maskUpdate, conflictDetected);
        }
    }
}