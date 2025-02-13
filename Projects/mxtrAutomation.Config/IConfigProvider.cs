using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mxtrAutomation.Config
{
    public interface IConfigProvider
    {
        object Get(Type t, string n);
    }
}
