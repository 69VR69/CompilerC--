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

namespace CompilerC__.NewFolder
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
            Accept("(");
            Accept(")");
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
                Accept("(");
                Node test = Expression();
                Accept(")");
                Node then = Instruction();

                if (Check("else"))
                    return new Node("cond", test, then, Instruction());
                else
                    return new Node("cond", test, then);
            }
            else if (Check("{"))
            {
                Node block = new Node("block");
                while (!Check("}"))
                {
                    block.Childs.Add(Instruction());
                }
                return block;
            }
            else
            {
                Node e = Expression();
                Accept(";");
                return new Node(e);
            }

        }

        private Node Expression()
        {
            return Execute();
        }

        private Node Prefixe()
        {
            if (Check("-"))
            {
                Node node = Prefixe();
                return node;
            }
            else if (Check("+"))
            {
                Node node = Prefixe();
                return node;
            }
            else if (Check("!"))
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
            if (Check("("))
            {
                Node node = Expression();
                Accept(")");
                return node;
            }
            else if (Check("const"))
            {
                Token token = LexicalScanner.Last;
                Node node = new(token, true);
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
            Node arg1 = Prefixe();
            DataRow row;
            string op;
            int i = 0;

            // to transform as in dtOperation
            while ((op = (string)((row = Utils.dtOperations.Rows[i])[LexicalScanner.Current.Type])) != null)
            {
                int prio = (int)row["prio"];

                if (prio >= pmin)
                {
                    LexicalScanner.NextToken();

                    Node arg2 = Execute(prio + ((bool)row["isLeftAsso"] ? 1 : 0));


                    arg1 = new Node(Utils.GetNodeType(op).Code, arg1, arg2);
                }
                else
                    break;
            }
            return arg1;
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
