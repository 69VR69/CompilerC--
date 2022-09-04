using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class GrammarGroup : Element
    {
        public string Code { get; set; }

        public char Sign { get; set; }

        public GrammarGroup(string code, char sign)
        {
            Code = code;
            Sign = sign;
        }

        public Node Execute(LexicalScanner lexicalScanner)
        {
            ElementGroup[]? egs = Utils.GetElementGroups(this);

            if (egs == null)
                Utils.PrintError("no_element_group", Code);

            foreach (ElementGroup eg in egs)
            {
                Node root = new Node();
                Node current = new Node();
                root.Childs.Add(current);

                foreach (Element e in eg.Elements)
                {
                    if (typeof(GrammarGroup).IsAssignableFrom(e.GetType()))
                    {
                        Node? newNode = Utils.GetGroup((GrammarGroup)e)?.Execute(lexicalScanner);

                        if (newNode == null)
                            Utils.PrintError("unrecognized_grammargroup", (GrammarGroup)e);

                        current.Childs.Add(newNode);
                        current = newNode;
                    }
                    else if (typeof(TokenType).IsAssignableFrom(e.GetType()))
                    {
                        if (lexicalScanner.Check((TokenType)e))
                        {
                            Token token = lexicalScanner.Current;
                            Node? newNode = new Node(token.NodeType, token.Line, token.Column);
                        }
                    }
                    else
                        Utils.PrintError("unrecognized_element", e);
                }
            }

            return null;
        }

        public Node Execute2(LexicalScanner lexicalScanner)
        {
            ElementGroup[]? egs = Utils.GetElementGroups(this);

            if (egs == null)
                Utils.PrintError("no_element_group", Code);

            foreach (ElementGroup eg in egs)
            {
                Node root = eg.Node.Clone();
                foreach (Element e in eg.Elements)
                {
                    if (IsTokenType(e))
                    {
                        TokenType tokenType = (TokenType)e;
                        if (lexicalScanner.Check(tokenType))
                        {
                            Token current = lexicalScanner.Last;
                            Node node = eg.GetNextNode();

                            if (tokenType.Code == "const" || tokenType.Code == "var")
                                node.Value = current.Value;

                            node.Line = current.Line;
                            node.Column = current.Column;
                        }
                    }
                    else if (IsGrammarGroup(e))
                    {
                        Node? newNode = Utils.GetGroup((GrammarGroup)e)?.Execute(lexicalScanner);

                        if (newNode == null)
                            Utils.PrintError("unrecognized_grammargroup", (GrammarGroup)e);
                    }
                    else
                        Utils.PrintError("");
                }
            }
        }

        private bool IsGrammarGroup(Element e)
        {
            return e.GetType().IsAssignableFrom(typeof(GrammarGroup));
        }
        private bool IsTokenType(Element e)
        {
            return e.GetType().IsAssignableFrom(typeof(TokenType));
        }
    }
}















