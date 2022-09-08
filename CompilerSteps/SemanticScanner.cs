using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;

namespace CompilerC__.CompilerSteps
{
    internal class SemanticScanner
    {
        public SyntaxScanner SyntaxScanner
        {
            get; set;
        }

        public SemanticScanner()
        {
            SyntaxScanner = new SyntaxScanner();
        }

        public Node SeS()
        {
            return SyntaxScanner.SS();
        }
    }
}
