using System;
using System.IO;
using System.Linq;

namespace PolySat
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"..\..\..\..\..\samples\Circular logical deadlock"; // SAT/UNSAT
            //string path = @"..\..\..\..\..\samples\uf20-91";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\uuf50-218\UUF50.218.1000";// ALL UNSATISFABLE
            string path = @"..\..\..\..\..\samples\test";
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
            foreach (var p in problem)
            {
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------------------------------------------------------------");
                Console.WriteLine();

                using var w = new StreamWriter($"{path}.out", false);
                int varCount = p.VariableCount;
                var store = new VectorStore(varCount);

                store.AddConstraints(p.Constraints);

                Console.WriteLine($"{ DateTime.Now} Loaded problem file {path}");

                var calculator = new VectorCalculator(store);
                var satisfable = calculator.IsSatisfable();

                if (!satisfable)
                {
                    Console.WriteLine($"{DateTime.Now} Problem file {path}. Resolution: UNSAT");
                    continue;
                }

                Console.WriteLine($"{DateTime.Now} Problem file {path}. Resolution: SATisfable");
                continue;
            }
        }
    }
}