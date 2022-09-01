using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__
{
    internal class TokenType
    {
        public string Code { get; set; }
        public List<string> MatchedCharacters { get; set; }

        public TokenType(string code, List<string> matchedCharacters)
        {
            Code = code;
            MatchedCharacters = matchedCharacters;
        }
    }
}
