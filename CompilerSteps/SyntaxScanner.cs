using CompilerC__.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.NewFolder
{
    internal class SyntaxScanner
    {
        public LexicalScanner LexicalScanner { get; set; }        

        public SyntaxScanner(LexicalScanner lexicalScanner)
        {
            LexicalScanner = lexicalScanner;
        }

        public Node SS()
        {
           Node? n =  Utils.GetGroup("General")?.Execute(LexicalScanner);
            return n; 
        }
    }
}
