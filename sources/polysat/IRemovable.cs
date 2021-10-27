using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolySat
{
    public interface IRemovable
    {
        void Remove();
        void Restore();
    }
}
