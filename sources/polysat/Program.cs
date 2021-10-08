using System;
using System.IO;
using System.Linq;

namespace PolySat
{
    class Program
    {
        static void Main(string[] args)
        {
            //string path = @"..\..\..\..\..\samples\Mikhail";// UNSATISFABLE
            //string path = @"..\..\..\..\..\samples\flat30-60";// ALL SATISFABLE
            string path = @"..\..\..\..\..\samples\uf20-91";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\uuf50-218\UUF50.218.1000";// ALL UNSATISFABLE
            //string path = @"..\..\..\..\..\samples\uf50-218";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\uf100-430";// ALL SATISFABLE
            //string path = @"..\..\..\..\..\samples\UUF250.1065.100";// ALL UNSATISFABLE
            //string path = @"..\..\..\..\..\samples\RTI_k3_n100_m429";

            //string path = @"C:\Books\Math\cnf";

            foreach (var f in Directory.GetFiles(path, "*.cnf")/*.Skip(2).Take(1)*/)
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
                using (var w = new StreamWriter($"{path}.out", false))
                {
                    var store = new StateStore(p.VariableCount, w);
                    w.WriteLine($"p cnf {p.VariableCount} 0");
 
                    store.AddConstraints(p.Constraints);

                    Console.WriteLine($"{ DateTime.Now} Loaded problem file {path}");
                    if (new ProblemCalculator(store).IsSatisfable())
                    {
                        Console.WriteLine($"{DateTime.Now} SAT");
                        w.WriteLine("c SATISFABLE");
                    }
                    else
                    {
                        Console.WriteLine($"{DateTime.Now} UNSAT");
                        w.WriteLine("c UNSATISFABLE");
                    }
                }
            }
        }
    }
}
