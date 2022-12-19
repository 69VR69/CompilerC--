using System.Xml.Linq;

using CompilerC__.Objects;
using CompilerC__.src;
using CompilerC__.src.Objects;

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
            if (Utils.debugMode)
                Console.WriteLine("\nSyntax scanning start !");

            Node node = General();

            if (Utils.debugMode)
                Console.WriteLine("Syntax scanning end !\n");

            return node;
        }

        private Node General()
        {
            // Check pattern #include <stdio.h>
            if (Check("preproc"))
            {
                string preprocName = LexicalScanner.Current.Value;

                if (preprocName == "include")
                {
                    LexicalScanner.NextToken();
                    Accept("lowChevron");

                    string libName = Path.GetFileNameWithoutExtension(LexicalScanner.Current.Value);
                    int libLine = LexicalScanner.Current.Line;

                    LexicalScanner.NextToken();
                    Accept("upChevron");

                    return new Node("lib", libName, libLine);
                }

                return new Node();
            }
            else
                return Function();
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

                    if (Check("star"))
                    {
                        declaration.Childs.Add(new("indirection", childs: new Node(LexicalScanner.Current, withValue: true)));
                    }
                    else if (Check("squareBracketIn"))
                    {
                        Accept("squareBracketOut");
                        declaration.Childs.Add(new("indirection", childs: new Node(LexicalScanner.Current, withValue: true)));
                    }
                    else
                    {
                        declaration.Childs.Add(new(LexicalScanner.Current, withValue: true));
                    }

                    LexicalScanner.NextToken();

                    while (Check("comma"))
                    {
                        Accept("int");

                        if (Check("star"))
                            declaration.Childs.Add(new("indirection", childs: new Node(LexicalScanner.Current, withValue: true)));
                        else
                            declaration.Childs.Add(new(LexicalScanner.Current, withValue: true));

                        LexicalScanner.NextToken();
                    }
                }
                Accept("parenthesisOut");

                Node instr = Instruction();

                return new Node("function", value: ident, line, declaration, instr);
            }
            else
            {
                Utils.PrintError("function_declaration_missing", true, LexicalScanner.Current.Line);
                return new Node();
            }
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
                    if (Check("star"))
                    {
                        declaration.Childs.Add(new("indirection", childs: new Node(LexicalScanner.Current, withValue: true)));
                    }
                    else if (Check("squareBracketIn"))
                    {
                        Accept("squareBracketOut");
                        declaration.Childs.Add(new("indirection", childs: new Node(LexicalScanner.Current, withValue: true)));
                    }
                    else
                    {
                        declaration.Childs.Add(new(LexicalScanner.Current, withValue: true));
                    }

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
            else if (Check("send"))
            {
                Accept("parenthesisIn");
                Node expr = Expression();
                Accept("parenthesisOut");
                Accept("semicolon");
                return new Node("send", expr);
            }
            else
            {
                Node e = Expression();
                Accept("semicolon");
                e.Childs.Add(new Node("semicolon"));
                return e;
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
                return new("sub", new Node("const", value: "0", 0), Prefixe());
            }
            else if (Check("plus"))
            {
                return Prefixe();
            }
            else if (Check("exclamation"))
            {
                return new("not", Prefixe());

            }
            else if (Check("ampersand"))
            {
                return new("addrOf", Prefixe());
            }
            else if (Check("star"))
            {
                return new("indirection", Prefixe());
            }
            else
            {
                Node node = Sufixe();
                return node;
            }
        }

        private Node Sufixe()
        {
            Node n = Atome();

            while (Check("squareBracketIn"))
            {
                Node index = Expression();
                Accept("squareBracketOut");

                n = new("indirection", new Node("add", n, index));
            }

            return n;
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
            else if (Check("receive"))
            {
                return new("receive");
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

            while (Utils.IsOperation(LexicalScanner?.Current?.Type))
            {
                Operation? op = Utils.GetOperation(LexicalScanner?.Current?.Type);

                if (op == null || op.Priority < pmin)
                    break;

                LexicalScanner.NextToken();

                prefixNode = new(op.NodeType?.Code, prefixNode, Execute(op.Priority + (op.IsLeftAssociate ? 1 : 0)));
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
