using System.Collections.Generic;
using System.Linq;

namespace PolySat.KernelBuilder
{
    public static class ConstituentExtensions
    {
        public static IEnumerable<Implicant> GetKernel(this IEnumerable<Constituent> constSet)
        {
            return constSet.Select(c => c.ToImplicantSet()).Aggregate((a, i) => a.Product(i).Reduce());
        }
        private static IEnumerable<Implicant> ToImplicantSet(this Constituent source)
        {
            // any constituent can be represented as set of implicants that each
            // of which contains a single inverted literal from the source
            return source.Select(x => new Implicant(new[] { -x }));
        }
    }
}
