using System;
using System.IO;

namespace PolySat
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\uf20-91")) // ALL SATISFABLE
            //foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\uuf50-218\UUF50.218.1000")) // ALL UNSATISFABLE
            //foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\uf50-218")) // ALL SATISFABLE
            //foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\uf100-430")) // ALL SATISFABLE
            //foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\UUF250.1065.100")) // ALL UNSATISFABLE
            //foreach(var f in Directory.GetFiles(@"..\..\..\..\..\samples\RTI_k3_n100_m429"))
            {
                Run(f);
            }

            //var path = @"..\..\..\..\..\samples\uf50-218\uf50-012.cnf";                   // SAT
            //var path = @"..\..\..\..\..\samples\uf100-430\uf100-0886.cnf";                  // SAT
            //var path = @"..\..\..\..\..\samples\UUF250.1065.100\uuf250-094.cnf"; // UNSAT
            //var path = @"..\..\..\..\..\samples\RTI_k3_n100_m429\RTI_k3_n100_m429_0.cnf"; //

            Console.ReadLine();
        }

        static void Run(string path)
        {
            var problem = ProblemLoader.Load(path);
            Console.WriteLine($"{ DateTime.Now} Loaded problem file {path}");
            if (new ProblemCalculator(problem).IsSatisfable())
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
