using System;
using System.IO;
using System.Linq;

namespace PolySat
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"..\..\..\..\..\samples\Circular logical deadlock"; // SAT/UNSAT
            //string path = @"..\..\..\..\..\samples\uf20-91";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\uuf50-218\UUF50.218.1000";// ALL UNSATISFABLE
            //string path = @"..\..\..\..\..\samples\uf50-218";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\uf100-430";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\flat30-60";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\UUF250.1065.100";// ALL UNSATISFABLE
            //string path = @"..\..\..\..\..\samples\RTI_k3_n100_m429";

            foreach (var f in Directory.GetFiles(path, "*.cnf"))
            {
                Run(f);
            }

            Console.ReadLine();
        }

        static void Run(string path)
        {
            var problem = ProblemLoader.Load(path);
            foreach(var p in problem)
            {
                using var w = new StreamWriter($"{path}.out", false);
                var store = new VectorStore(p.VariableCount, w);

                store.AddConstraints(p.Constraints);

                Console.WriteLine($"{ DateTime.Now} Loaded problem file {path}");

                var satisfable = new VectorCalculator(store, w).IsSatisfable();

                w.Flush();

                if (satisfable)
                {
                    Console.WriteLine($"{DateTime.Now} SAT");
                }
                else
                {
                    Console.WriteLine($"{DateTime.Now} UNSAT");
                }
            }
        }
    }
}
