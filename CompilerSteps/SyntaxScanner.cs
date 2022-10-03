using System.Xml.Linq;

using CompilerC__.Objects;

namespace CompilerC__.CompilerSteps
{
    internal class SyntaxScanner
    {
        public LexicalScanner LexicalScanner { get; set; }

        public SyntaxScanner()
        {
            LexicalScanner = new LexicalScanner();
        }

        // General
        public Node SS()
        {

            Console.WriteLine("\n\nSyntax scanning start !\n");

            Node node = Function();

            Console.WriteLine("\n\nSyntax scanning end !\n");

            return node;
        }

        private Node Function()
        {
            if (Check("int"))
            {
                Node declaration = new Node();

                Token identToken = LexicalScanner.Current;
                string ident = identToken.Value;
                int line = identToken.Line;
                LexicalScanner.NextToken();

                Accept("parenthesisIn");
                if (Check("int"))
                {
                    declaration = new Node("declaration");
                    Token temp;
                    temp = LexicalScanner.Current;
                    declaration.Childs.Add(new(temp, withValue: true));

                    while (Check("comma"))
                    {
                        Accept("int");
                        temp = LexicalScanner.Current;
                        declaration.Childs.Add(new(temp, withValue: true));
                    }
                }
                Accept("parenthesisOut");

                Node instr = Instruction();

                return new Node("function", value: ident, line, declaration, instr);
            }
            else
                return Instruction();
        }

        private Node Instruction()
        {
            if (Check("if"))
            {
                Accept("parenthesisIn");
                Node test = Expression();
                Accept("parenthesisOut");
                Node then = Instruction();

                if (Check("else"))
                    return new("cond", test, then, Instruction());
                else
                    return new("cond", test, then);
            }
            else if (Check("bracketIn"))
            {
                Node block = new("block");
                while (!Check("bracketOut"))
                {
                    block.Childs.Add(Instruction());
                }
                return block;
            }
            else if (Check("int"))
            {
                Node declaration = new("declaration");

                do
                {
                    Token ident = LexicalScanner.Current;
                    declaration.Childs.Add(new("var", ident.Value, ident.Line));
                    LexicalScanner.NextToken();
                } while (Check("comma"));

                Accept("semicolon");
                return declaration;
            }
            else if (Check("break"))
            {
                Accept("semicolon");
                return new("break");
            }
            else if (Check("continue"))
            {
                Accept("semicolon");
                return new("continue");
            }
            else if (Check("while"))
            {
                Accept("parenthesisIn");
                Node test = Expression();
                Accept("parenthesisOut");
                Node then = Instruction();

                return new("loop", new Node("continueLabel"), new Node("cond", test, then, new Node("break")));
            }
            else if (Check("do"))
            {
                Node i = Instruction();

                Accept("while");
                Accept("parenthesisIn");
                Node test = Expression();
                Accept("parenthesisOut");
                Accept("semicolon");

                return new Node("loop", new Node("continueLabel"), i, new Node("cond", new Node("not", test), new Node("break")));
            }
            else if (Check("for"))
            {
                Accept("parenthesisIn");
                Node init = Expression();
                Accept("semicolon");
                Node test = Expression();
                Accept("semicolon");
                Node step = Expression();
                Accept("parenthesisOut");
                Node then = Instruction();

                return new Node("seq", init, new Node("loop", then, new Node("continueLabel"), step, new Node("cond", new Node("not", test), new Node("break"))));
            }
            else if (Check("return"))
            {
                Node expr = Expression();
                Accept("semicolon");
                return new Node("return", expr);
            }
            else
            {
                Node e = Expression();
                Accept("semicolon");
                return new Node(e);
            }

        }

        private Node Expression()
        {
            return Execute();
        }

        private Node Prefixe()
        {
            if (Check("minus"))
            {
                Node node = Prefixe();
                return node;
            }
            else if (Check("plus"))
            {
                Node node = Prefixe();
                return node;
            }
            else if (Check("exclamation"))
            {
                Node node = Prefixe();
                return node;
            }
            else
            {
                Node node = Sufixe();
                return node;
            }
        }

        private Node Sufixe()
        {
            return Atome();
        }

        private Node Atome()
        {
            if (Check("parenthesisIn"))
            {
                Node node = Expression();
                Accept("parenthesisOut");
                return node;
            }
            else if (Check("const"))
            {
                Token token = LexicalScanner.Last;
                Node node = new(token, withValue: true);
                return node;
            }
            else if (Check("ident"))
            {
                Token token = LexicalScanner.Last;
                if (Check("parenthesisIn"))
                {

                    if (Check("parenthesisOut"))
                        return new("call", token.Value, token.Line);
                    else
                    {
                        Node declaration = new("declaration", Expression());

                        while (Check("comma"))
                            declaration.Childs.Add(Expression());

                        Accept("parenthesisOut");

                        return new("call", token.Value, token.Line)
                        {
                            Childs = declaration.Childs
                        };
                    }
                }
                else
                    return new(token, withValue: true);
            }
            else if (Check("eos"))
            {
                return new("eos");
            }
            else
            {
                Token token = LexicalScanner.Current;
                Utils.PrintError("unrecognized_token", true, token.Type, token.Line);
                return null;
            }
        }

        private Node Execute(int pmin = 0)
        {
            Node prefixNode = Prefixe();
            Operation? op;

            while ((op = Utils.GetOperation(LexicalScanner?.Current?.Type)) != null)
            {
                if (op.Priority < pmin)
                    break;

                LexicalScanner.NextToken();
                prefixNode = new Node(op.NodeType?.Code, prefixNode, Execute(op.Priority + (op.IsLeftAssociate ? 1 : 0)));
            }

            return prefixNode;
        }

        private bool Check(string tokenType)
        {
            return LexicalScanner.Check(Utils.GetTokenType(tokenType));
        }
        private void Accept(string tokenType)
        {
            LexicalScanner.Accept(Utils.GetTokenType(tokenType));
        }
    }
}
