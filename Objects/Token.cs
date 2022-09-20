using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class Token
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }

        public Token(string type, string value, int line =0, int column=0)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
        }
    }
}
