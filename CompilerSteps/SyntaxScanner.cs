using CompilerC__.Objects;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using static System.Net.Mime.MediaTypeNames;

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
            Accept("int");
            Accept("main");
            Accept("parenthesisIn");
            Accept("parenthesisOut");
            Node node = Function();
            Accept("eos");
            return node;
        }

        private Node Function()
        {
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
                    return new ("cond", test, then, Instruction());
                else
                    return new ("cond", test, then);
            }
            else if (Check("bracketIn"))
            {
                Node block = new ("block");
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
                    declaration.Childs.Add(new("var", ident.Value, ident.Line, ident.Column));
                    LexicalScanner.NextToken();
                } while (Check("comma"));


                Accept("semicolon");
                return declaration;
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
                Node node = new(token, withValue: true);
                return node;
            }
            else
            {
                Token token = LexicalScanner.Current;
                Utils.PrintError("unrecognized_token", true, token.Type, token.Line, token.Column);
                return null;
            }
        }

        private Node Execute(int pmin = 0)
        {
            Node prefixNode = Prefixe();
            Operation? op;

            while ((op = Utils.GetOperation(LexicalScanner?.Current?.Type)) != null)
            {
                if (op.Priority >= pmin)
                {
                    LexicalScanner.NextToken();

                    prefixNode = new Node(op.NodeType?.Code, prefixNode, Execute(op.Priority + (op.IsLeftAssociate ? 1 : 0)));
                }
                else
                    break;
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
