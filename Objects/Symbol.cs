using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects.Types;

namespace CompilerC__.Objects
{
    internal class Symbol
    {
        public SymbolType Type { get; set; }

        public string Ident { get; set; }

        public int Address { get; set; }

        public int? NbParam { get; set; }

        public Symbol(SymbolType type, string ident, int address = -1, int? nbParam = null)
        {
            Type = type;
            Ident = ident;
            Address = address;
            NbParam = nbParam;
        }

        public override string ToString()
        {
            return $"{Type}";
        }
    }
}
