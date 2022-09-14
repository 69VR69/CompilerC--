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
        public SyntaxScanner SyntaxScanner { get; set; }
        public Stack<HashSet<Symbol>> SymbolTable { get; set; }

        public SemanticScanner()
        {
            SyntaxScanner = new SyntaxScanner();
            SymbolTable = new Stack<HashSet<Symbol>>();
        }

        public Node SeS()
        {
            Node n = SyntaxScanner.SS();
            Utils.nbVar = 0;
            SemNode(n);
            return n;
        }

        private void SemNode(Node n)
        {
            switch (n.Type)
            {
                default:
                    foreach (Node c in n.Childs)
                        SemNode(c);
                    break;

                case "block":
                    StartBlock();
                    foreach (Node c in n.Childs)
                        SemNode(c);
                    EndBlock();
                    break;

                case "var":
                    if (n.Value == null)
                        Utils.PrintError("var_without_ident", arg: n.Value);

                    n.Address = SearchSymbol((int)n.Value).Address; // check type
                    break;

                case "assign":
                    if (n.Childs[0].Type != "var")
                        Utils.PrintError("assign_to_non_var", arg: n.Childs[0].Value);
                    else
                        foreach (Node c in n.Childs)
                            SemNode(c);
                    break;

                case "declaration":
                    foreach (Node c in n.Childs)
                    {
                        if (c.Value == null)
                            Utils.PrintError("var_without_ident", arg: c.Value);

                        Symbol s = Declare((int)c.Value);

                        s.Type = Utils.GetSymbolType("var");
                        s.Address = Utils.nbVar;
                        Utils.nbVar++;
                    }
                    break;
            }
        }

        private Symbol Declare(int value)
        {
            throw new NotImplementedException();
        }

        private Symbol SearchSymbol(int ident)
        {
            throw new NotImplementedException();
        }

        private void EndBlock()
        {
            throw new NotImplementedException();
        }

        private void StartBlock()
        {
            throw new NotImplementedException();
        }
    }
}
