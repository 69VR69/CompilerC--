using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace CompilerC__.Objects.Types
{
    internal class TokenType
    {
        public string Code { get; set; }
        public int Order { get; set; }
        public List<char>? MatchedCharacters { get; set; }
        public Regex? Regex { get; set; }

        public TokenType(string code, int order = 0, string regex = "")
        {
            Code = code;
            Order = order;
            Regex = new Regex($"^{regex}$");
        }
        
        public TokenType(string code, int order = 0, params char[] matchedCharacters)
        {
            Code = code;
            Order = order;
            MatchedCharacters = matchedCharacters.ToList();
        }

        public bool IsMatch(string s)
        {
            if (MatchedCharacters != null && MatchedCharacters.Count > 0)
            {
                if (MatchedCharacters.Any(c => c.ToString() == s))
                    return true;
            }

            if (Regex != null)
            {
                if (Regex.IsMatch(s))
                    return true;
            }

            return false;
        }

        public string AddSpaceAround(string s)
        {
            if (MatchedCharacters != null && MatchedCharacters.Count > 0)
            {
                foreach (var t in MatchedCharacters)
                    s = s.Replace(t.ToString(), $" {t} ");
            }

            if (Regex != null)
            {
                foreach (var t in Regex.Matches(s))
                    s = Regex.Replace(s, $" {t} ");
            }

            return s;
        }
    }
}
