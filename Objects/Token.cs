using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;

namespace CompilerC__
{
    internal class Token
    {
        public string Type { get; set; }
        public int Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(string type, int value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }
    }
}
