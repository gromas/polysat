using Microsoft.VisualStudio.TestTools.UnitTesting;
using PolySat.KernelBuilder;
using System.Collections.Generic;
using System.Linq;

namespace Polysat.KernelBuilder.Tests
{
    [TestClass]
    public class DNFBuilderTests
    {
        [TestMethod]
        public void BuildDNF()
        {
            // 4-SAT CNF SATinsfable
            var constSet = new Constituent[]
            {
                new (new Constituent( new [] { -1, -2, 3, 4 })),
                new (new Constituent( new [] { -1, 2, -3, -4 })),
                new (new Constituent( new [] { -1, 2, 3, -4 })),
                new (new Constituent( new [] { 1, -2, -3, -4 })),
                new (new Constituent( new [] { 1, -2, 3, -4 })),
                new (new Constituent( new [] { 1, -2, 3, 4 })),
                new (new Constituent( new [] { 1, 2, 3, -4 })),
            };

            // build kernel for 4-SAT CNF
            var kernel = constSet.GetKernel().ToHashSet();

            var expected = new HashSet<Implicant>
            {
                new Implicant(new[]{ 1, 2, -3 }),
                new Implicant(new[]{ 2, 4 }),
                new Implicant(new[]{ -3, -2, -1 }),
                new Implicant(new[]{ -3, 4 }),
                new Implicant(new[]{ -4, -2, -1 }),
            };

            // check results
            Assert.IsNotNull(kernel, "kernel is null");
            // implicants count must be equal to expected
            Assert.AreEqual(kernel.Count, expected.Count, "count invalid");
            // implicants must be equal to expected
            // WARNING: SetEquals is literals order sensitive
            Assert.IsTrue(kernel.SetEquals(expected), "output not expected");
        }

        [TestMethod]
        public void BuildUnsatDNF()
        {
            // 3-SAT CNF UNSATisfable
            var constSet = new Constituent[]
            {
                new (new Constituent( new [] { 1, 2 })),
                new (new Constituent( new [] { 1, -2 })),
                new (new Constituent( new [] { -1, 3 })),
                new (new Constituent( new [] { -1, -3 })),
            };

            // build kernel for 3-SAT CNF
            var kernel = constSet.GetKernel().ToHashSet();

            // check results
            Assert.IsNotNull(kernel, "kernel is null");
            // UNSATisfable kernel contains zero implicants
            Assert.AreEqual(kernel.Count, 0, "count invalid");
        }
    }
}
