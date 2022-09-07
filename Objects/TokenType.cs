using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using CompilerC__.Objects;

namespace CompilerC__
{
    internal class TokenType : Element
    {
        public string Code { get; set; }
        public List<char>? MatchedCharacters { get; set; }
        public Regex? Regex { get; set; }

        public TokenType(string code, string regex)
        {
            Code = code;
            Regex = new Regex($"^{regex}$");
        }

        public TokenType(string code, params char[] matchedCharacters)
        {
            Code = code;
            MatchedCharacters = matchedCharacters.ToList<char>();
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

        public string ReplaceInCharacters(string s)
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
