using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CompilerC__.Objects.Types
{
    internal class ComposedTokenType : TokenType
    {
        public List<TokenType> TokenTypes { get; set; }

        public ComposedTokenType(string code, int order = 0, params TokenType[] tokenTypes) : base(code, order)
        {
            TokenTypes = new List<TokenType>();
            TokenTypes.AddRange(tokenTypes.ToList());
        }
        
    }
}
