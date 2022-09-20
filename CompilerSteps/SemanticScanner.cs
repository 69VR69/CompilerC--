using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;
using CompilerC__.Objects.Types;

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
            

            Console.WriteLine("\n\nSemantic scanning start !\n");
            
            SemNode(n);
            
            Console.WriteLine("\nSemantic scanning end !\n\n");

            
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

                case "ident":
                    if (n.Value == null)
                        Utils.PrintError("var_without_ident", arg: n.Value);

                    n.Address = SearchSymbol(n.Value, Utils.GetSymbolType("var")).Address;
                    break;

                case "assign":
                    if (n.Childs[0].Type != "ident")
                        Utils.PrintError("assign_to_non_var", arg: n.Childs[0].Type);
                    else
                        foreach (Node c in n.Childs)
                            SemNode(c);
                    break;

                case "declaration":
                    foreach (Node c in n.Childs)
                    {
                        if (c.Value == null)
                            Utils.PrintError("var_without_ident", arg: c.Value);

                        Declare(c.Value, Utils.GetSymbolType("var"));
                    }
                    break;
            }
        }

        private Symbol Declare(string ident, SymbolType type)
        {
            HashSet<Symbol> lastTable = SymbolTable.Last();

            if (lastTable.Any(s => s.Ident == ident))
                Utils.PrintError("symbol_already_declared", arg: ident);

            lastTable.Add(new(type, ident, Utils.nbVar));
            Utils.nbVar++;

            return SymbolTable.Last<HashSet<Symbol>>().Last<Symbol>();
        }

        private Symbol SearchSymbol(string ident, SymbolType type)
        {
            foreach (HashSet<Symbol> table in SymbolTable)
            {
                Symbol? symbol = table.FirstOrDefault(s => s.Ident == ident && s.Type == type);

                if (symbol != null)
                    return symbol;
            }

            Utils.PrintError("unrecognized_symbol", true, ident);
            return null;
        }

        private void EndBlock()
        {
            SymbolTable.Pop();
        }

        private void StartBlock()
        {
            SymbolTable.Push(new HashSet<Symbol>());
        }
    }
}
