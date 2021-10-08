using System;
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
        public static CombinationSet Load(string path)
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

                            var stateStore = new StateStore(variableCount);
                            var cobinations = new CombinationSet(stateStore);

                            ParseSet(r, cobinations);
                            return cobinations;
                        }
                    }
                }
            }
            throw new Exception("can't load problem");
        }

        private static void ParseSet(StreamReader r, CombinationSet combinations)
        {
            if (!r.EndOfStream)
            {
                foreach (string line in Regex.Split(r.ReadToEnd(), @"\s0"))
                {
                    if (line.Trim() != "" && line.Trim() != "%")
                    {
                        var literals = line.Trim().Split(" ").Select(value => int.Parse(value)).ToArray();

                        if (literals.Length == 3)
                        {
                            combinations.AddConstraint(literals[0], literals[1], literals[2]);
                        }
                        else if (literals.Length == 2)
                        {
                            combinations.AddConstraint(literals[0], literals[1]);
                        }
                        else throw new Exception("literals count");
                    }
                }
            }
        }
    }
}
