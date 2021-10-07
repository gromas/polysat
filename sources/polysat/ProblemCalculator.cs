using System;
using System.Collections.Generic;
using System.Linq;

namespace PolySat
{
    public class ProblemCalculator
    {
        const byte NotSet = 2;

        private readonly CombinationSet combinations;
        public ProblemCalculator(CombinationSet combinations)
        {
            this.combinations = combinations;
        }

        public bool IsSatisfable()
        {
            bool changed;
            bool satisfable;

            /// LOOP1 - interate while each combination contains least one combination state and state changes detected
            /// Theory maximum: O(8(n^3-3n^2+2n))
            /// Practical: O(5)
            /// Each iteration removes least one combination state from existing possible states
            /// If combination states wasn't removed in current iteration then breaks the loop and return
            /// SAT if each combination contains least one combination state
            do
            {
                changed = false;
                satisfable = false;

                // LOOP2: iterate over all posible combination states of all combinations
                // Theory maximum: O(8(n^3-3n^2+2n))
                // Practical: O(8(n^3-3n^2+2n))
                // Each iteration 
                foreach (Combination c in combinations.All)
                {
                    satisfable = false;
                    // up to 8 posible combination states
                    var states = combinations.PosibleStates(c).ToArray();
                    // for each combination state we must check than exists consistent solution (no conflicts between mask and states)
                    foreach (CombinationState s in states)
                    {
                        satisfable = true;
                        // create combination state mask size of n
                        CombinationMask mask = new CombinationMask(combinations.VariablesCount);

                        // set values for known variabless
                        mask[c[0]] = s[c[0]];
                        mask[c[1]] = s[c[1]];
                        mask[c[2]] = s[c[2]];

                        // test combination state for consistency with all other combinations
                        if (!DepthSearch(mask))
                        {
                            changed = true;
                            // if for current combination state not exists consisten statemask
                            // with other combinations then current combination state is a constraint
                            combinations.AddConstraint(
                                s[c[0]] == 1 ? c[0] : -c[0],
                                s[c[1]] == 1 ? c[1] : -c[1],
                                s[c[2]] == 1 ? c[2] : -c[2]);
                            // if no more states in any combination then no solutions exists
                            if (combinations.PosibleStates(c).Count() == 0) return false;
                        }
                    }
                }
            } while (changed);

            return satisfable;
        }

        /// <summary>
        /// Search by mask in the depths
        /// </summary>
        private bool DepthSearch(CombinationMask mask)
        {
            bool changed;
            // reduce mask
            // MAX: O(n^3-3n^2+2n) (if exists no more than one change for each iteration and all iterations has changes)
            do
            {
                changed = false;
                // sequence of all unresolved combinations
                var unsolved = mask.UnresolvedCombinations;
                // find masked conflicts : MAX O((n-3)^3)
                foreach (Combination c in unsolved)
                {
                    // выбираем состояния сочетания, соответствующие маске
                    CombinationState[] states =
                        combinations.PosibleStates(c)
                        .Where(s =>
                        (mask[c[0]] == NotSet || mask[c[0]] == s[c[0]]) &&
                        (mask[c[1]] == NotSet || mask[c[1]] == s[c[1]]) &&
                        (mask[c[2]] == NotSet || mask[c[2]] == s[c[2]])).ToArray();

                    switch (states.Length)
                    {
                        case 0:
                            // no states compatible with current mask -> no solutions
                            return false;
                        case 1:
                            // single state found -> update mask and mark iteration has_changes
                            changed |= ApplyState(mask, c, states[0]);
                            break;
                        case 2:
                            // two states found -> if both contains equals variable values then update mask
                            for (int i = 0; i < 3; i++)
                            {
                                if (mask[c[i]] == NotSet && states[0][c[i]] == states[1][c[i]])
                                {
                                    changed = true;
                                    mask[c[i]] = states[0][c[i]];
                                }
                            }
                            break;
                        default:
                            // multiply states are compatible -> no changes
                            break;
                    }
                }
            } while (changed);
            // conflicts no longer exist -> returns true
            return true;
        }

        // Apply single state to mask
        private bool ApplyState(CombinationMask mask, Combination c, CombinationState s)
        {
            bool changed = false;
            // single state found -> update mask
            for (int i = 0; i < 3; i++)
            {
                if (mask[c[i]] == NotSet)
                {
                    changed = true;
                    mask[c[i]] = s[c[i]];
                }
            }
            return changed;
        }
    }
}