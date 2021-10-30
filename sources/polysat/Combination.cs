using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    /// <summary>
    /// Представляет сочетание по трём переменным
    /// </summary>
    public struct Combination
    {
        public readonly (int x0, int x1, int x2) Index;
        public Combination((int x0, int x1, int x2) index) => Index = index;
    }
}
