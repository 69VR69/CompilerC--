using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;

namespace CompilerC__.NewFolder
{
    internal class SemanticScanner
    {
        public SyntaxScanner SyntaxScanner
        {
            get; set;
        }

        public SemanticScanner(SyntaxScanner syntaxScanner)
        {
            SyntaxScanner = syntaxScanner;
        }

        public Node SeS()
        {
            return SyntaxScanner.SS();
        }
    }
}
