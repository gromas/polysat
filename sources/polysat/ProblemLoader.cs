using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace PolySat
{
    /// <summary>
    /// Loads CNF into new CombinationSet
    /// </summary>
    public class ProblemLoader
    {
        public struct Problem
        {
            public int VariableCount;
            public IEnumerable<int[]> Constraints;
        }

        public static IEnumerable<Problem> Load(string path)
        {
            using (var s = File.OpenRead(path))
            {
                using (StreamReader r = new StreamReader(s))
                {
                    Regex rp = new Regex(@"^p\s+cnf\s+(?<varsCount>[0-9]+)\s+(?<clauseCount>[0-9]+)");
                    Regex rc = new Regex(@"((?<var>[0-9]+)\s+)+\s+0");

                    while (!r.EndOfStream)
                    {
                        var l = r.ReadLine().TrimStart();
                        if (l.StartsWith('c'))
                            continue;
                        var mrp = rp.Match(l);
                        if (mrp.Success)
                        {
                            var variableCount = int.Parse(mrp.Groups[1].Value);

                            yield return new Problem { VariableCount = variableCount, Constraints = ParseSet(r) };
                            yield break;
                        }
                    }
                }
            }
            //throw new Exception("can't load problem");
        }

        private static IEnumerable<int[]> ParseSet(StreamReader r)
        {
            if (!r.EndOfStream)
            {
                foreach (string line in Regex.Split(r.ReadToEnd(), @"\s0"))
                {
                    if (line.Trim() != "" && line.Trim() != "%")
                    {
                        var literals = line.Trim().Split(" ").Select(value => int.Parse(value)).ToArray();

                        if (literals.Length != 2 && literals.Length != 3)
                        {
                            throw new Exception("literals count");
                        }

                        yield return literals;
                    }
                }
            }
        }
    }
}
