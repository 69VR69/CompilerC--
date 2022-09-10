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

        public int Ident { get; set; }

        public byte[] Address { get; set; }

        public Symbol(SymbolType type, int ident, byte[] address)
        {
            Type = type;
            Ident = ident;
            Address = address;
        }
    }
}
