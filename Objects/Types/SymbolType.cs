using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects.Types
{
    internal class SymbolType
    {
        public string Code { get; set; }
        
        public SymbolType(string code)
        {
            Code = code;
        }
    }
}
