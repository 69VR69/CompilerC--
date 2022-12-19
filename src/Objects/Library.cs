using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;

namespace CompilerC__.src.Objects
{
    internal class Library
    {
        public string Name { get; set; }
        public HashSet<Symbol> SymbolTable { get; set; }

        public StringBuilder AssemblyCode { get; set; }

        public Library(string name)
        {
            Name = name;
            SymbolTable = new HashSet<Symbol>();
            AssemblyCode = new StringBuilder();
        }
    }
}
