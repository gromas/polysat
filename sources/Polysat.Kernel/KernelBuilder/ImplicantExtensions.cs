using System.Collections.Generic;
using System.Linq;

namespace PolySat.KernelBuilder
{
    public static class ImplicantExtensions
    {
        public static IEnumerable<Implicant> Product(this IEnumerable<Implicant> implSet, IEnumerable<Implicant> outer)
        {
            // distinct all compatible combinations
            return implSet.SelectMany(impl => impl.CombineWith(outer)).ToHashSet();
        }
        public static IEnumerable<Implicant> Reduce(this IEnumerable<Implicant> implSet)
        {
            // eliminate tautology
            return implSet.Where(i => !i.IsTautologyOf(implSet)).Distinct();
        }
        private static IEnumerable<Implicant> CombineWith(this Implicant impl, IEnumerable<Implicant> other)
        {
            // combine implicant with all compatible others
            return other.Where(o => o.IsCompatibleTo(impl)).Select(o => new Implicant(impl.Union(o)));
        }
        private static bool IsCompatibleTo(this Implicant impl, Implicant other)
        {
            return !impl.Any(x => other.Contains(-x));
        }
        private static bool IsTautologyOf(this Implicant impl, IEnumerable<Implicant> implSet)
        {
            // impl is tautology of implSet if implSet contains any implicant than is a proper subset of impl
            return implSet.Any(outer => outer.IsProperSubsetOf(impl));
        }
    }
}
