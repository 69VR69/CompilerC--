using CompilerC__.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.NewFolder
{
    internal class SyntaxScanner
    {
        public LexicalScanner LexicalScanner { get; set; }        

        public SyntaxScanner(LexicalScanner lexicalScanner)
        {
            LexicalScanner = lexicalScanner;
        }

        // General
        public Node SS()
        {
            Node node = Function();
            LexicalScanner.Accept(Utils.GetTokenType("eos"));
            return node;
        }

        private Node Function()
        {
            return Instruction();
        }
        
        private Node Instruction()
        {
            //if(Check(""))
            return Expression();
        }
        
        private Node Expression()
        {
            return Prefixe();
        }
        
        private Node Prefixe()
        {
            return Sufixe();
        }

        private Node Sufixe()
        {
            return Atome();
        }

        private Node Atome()
        { 
            return null; 
        }

        private bool Check(string tokenType)
        {
            return LexicalScanner.Check(Utils.GetTokenType(tokenType));
        }
    }
}
