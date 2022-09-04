using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__
{
    internal class LexicalScanner
    {
        public Token Current { get; set; }
        public Token Last { get; set; }
        public List<Token> TokenBuffer { get; set; }
        public List<string> FileLines { get; set; }
        public int CurrentLine { get; set; }

        public LexicalScanner(List<string> fileLines)
        {
            FileLines = fileLines;
            CurrentLine = 0;
            TokenBuffer = new List<Token>();
            Current = new Token("eos", 0, 0, 0);
            Last = Current;
        }

        public List<Token> Next()
        {
            if (CurrentLine >= FileLines.Count)
                return new List<Token> { new Token(Utils.tokenTypes.Where(t => t.Code == "eos").Select(t => t).ToArray()[0].Code, 0, 0, 0) };

            string fileLine = FileLines[CurrentLine];
            CurrentLine++;
            string tokenType = string.Empty;
            int tokenValue = 0;
            int nbColumn = 0;
            char[] spacesDelemiter = Utils.tokenTypes.Where(t => t.Code == "space" && t.MatchedCharacters != null).SelectMany(t => t.MatchedCharacters).ToArray();
            TokenType[] ignored = Utils.GetTokenType("comment", "preproc");
            TokenType[] tokenTypes = Utils.tokenTypes.Where(t => t.Code != "space" && t.Code != "comment" && t.Code != "preproc").Select(t => t).ToArray();

            // Reformat the line
            string formattedFileLine = FormatLine(fileLine);

            // Split the line in block of tokens
            List<string> blocks = formattedFileLine.Split(spacesDelemiter, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Token>? foundToken = new List<Token>();
            foreach (string b in blocks)
            {
                // Stop if found a ignore character
                if (ignored.Any(i => i.IsMatch(b)))
                    break;

                // Recognize the token
                List<TokenType> possibleTokenType = tokenTypes.Where(t => t.IsMatch(b)).ToList();
                if (possibleTokenType.Count > 1 && Utils.debugMode)
                    Console.WriteLine($"possybility for block \"{b}\" : \"{possibleTokenType?.Select(t => t.Code)?.Aggregate((a, b) => $"{a} {b}")}\"");
                tokenType = (possibleTokenType.Count > 0) ? possibleTokenType.First().Code : null;

                if (!string.IsNullOrWhiteSpace(tokenType))
                    foundToken.Add(new Token(tokenType, tokenValue, CurrentLine, nbColumn));
            }

            if (foundToken != null && foundToken.Count > 0 && Utils.debugMode)
            {
                Console.WriteLine($"initial line : {fileLine}");
                Console.WriteLine($"token found : {foundToken.Select(t => t.Type).Aggregate((a, b) => $"{a} {b}")}\n");
                TokenBuffer.AddRange(foundToken);
            }

            return foundToken;
        }

        private string FormatLine(string fileLine) // TODO : complete
        {
            return fileLine
                .Replace(";", " ; ")
                .Replace("(", " ( ")
                .Replace(")", " ) ")
                .Replace("#", " # ");
        }
        public Token NextToken()
        {
            Last = Current;
            Current = (TokenBuffer.Count <= 0) ? Next()[0] : TokenBuffer.First();
            TokenBuffer.RemoveAt(0);

            return Last;
        }

        public bool Check(TokenType type)
        {
            if (Current.Type == type.Code)
            {
                NextToken();
                return true;
            }
            return false;

        }
        public void Accept(TokenType type)
        {
            if (!Check(type))
                Utils.PrintError("");
        }

    }
}
