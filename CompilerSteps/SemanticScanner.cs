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
            SymbolTable.Push(new HashSet<Symbol>());
        }

        public Node SeS()
        {
            Node n = SyntaxScanner.SS();
            Utils.nbVar = 0;


            Console.WriteLine("\n\nSemantic scanning start !\n");

            SemNode(n);

            // function node contain the number of variables in the function sub by number of parameters
            n.Address = Utils.nbVar - n.Childs[0].Childs.Count;

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

                case "function":

                    Symbol s = Declare(n, Utils.GetSymbolType("func"));

                    StartBlock();

                    if (s == null)
                        Utils.PrintError("function_already_exist", true, n.Value);

                    foreach (Node c in n.Childs)
                        SemNode(c);

                    EndBlock();
                    break;

                case "call":
                    s = SearchSymbol(n.Value, Utils.GetSymbolType("func"));

                    if (s == null)
                        Utils.PrintError("function_not_found", true, n.Value);

                    Console.WriteLine("function " + n + " found, expected \n" + s.NbParam);

                    if (s.NbParam != n.Childs.Count)
                        Utils.PrintError("wrong_number_of_parameters", true, n.Value);

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

                        Declare(c, Utils.GetSymbolType("var"));
                    }
                    break;
            }
        }

        private Symbol Declare(Node node, SymbolType type)
        {
            HashSet<Symbol> lastTable = SymbolTable.Last();
            string ident = node.Value;

            if (lastTable.Any(s => s.Ident == ident))
                Utils.PrintError("symbol_already_declared", arg: ident);
            if (type == Utils.GetSymbolType("var"))
            {
                lastTable.Add(new(type, ident, address: Utils.nbVar));
                Utils.nbVar++;
            }
            else if (type == Utils.GetSymbolType("func"))
                lastTable.Add(new(type, ident, nbParam: node.Childs[0].Childs.Count));

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

        private void StartBlock() => SymbolTable.Push(new HashSet<Symbol>());
        private void EndBlock() => SymbolTable.Pop();
    }
}
