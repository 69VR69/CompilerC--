using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects;
using CompilerC__.Objects.Types;

namespace CompilerC__.CompilerSteps
{
    internal class LexicalScanner
    {
        public Token Current { get; set; }
        public Token Last { get; set; }
        public List<Token> TokenBuffer { get; set; }
        public List<string>? FileLines { get; set; }
        public int CurrentLine { get; set; }

        private readonly char[] spacesDelemiter = Utils.tokenTypes.Where(t => t.Code == "space" && t.MatchedCharacters != null).SelectMany(t => t.MatchedCharacters).ToArray();
        private readonly TokenType[] ignored = Utils.GetTokenType("comment", "preproc");
        private readonly TokenType[] tokenTypes = Utils.tokenTypes.Where(t => t.Code != "space" && t.Code != "comment" && t.Code != "preproc").Select(t => t).ToArray();

        public LexicalScanner()
        {
            FileLines = null;
            CurrentLine = 0;
            TokenBuffer = new List<Token>();
            Current = new Token("eos", 0, 0, 0);
            Last = Current;
        }

        private List<Token> Next()
        {
            if (CurrentLine >= FileLines.Count)
                return new List<Token> { new Token(Utils.tokenTypes.Where(t => t.Code == "eos").Select(t => t).ToArray()[0].Code, 0, 0, 0) };

            string fileLine = FileLines[CurrentLine];
            CurrentLine++;
            string tokenType = string.Empty;
            int tokenValue = 0;
            int nbColumn = 0;

            if (Utils.debugMode)
            {
                Console.WriteLine($"initial line : {fileLine}");
            }

            // Reformat the line
            string formattedFileLine = FormatLine(fileLine);

            // Split the line in block of tokens
            List<string> blocks = formattedFileLine.Split(spacesDelemiter, StringSplitOptions.RemoveEmptyEntries).ToList();

            List<Token>? foundToken = new();
            foreach (string b in blocks)
            {
                // Stop if found a ignore character
                if (ignored.Any(i => i.IsMatch(b)))
                    break;

                // Recognize the token
                List<TokenType> possibleTokenType = tokenTypes.Where(t => t.IsMatch(b)).OrderByDescending(t=> t.Order).ToList();
                if (possibleTokenType.Count > 1 && Utils.debugMode)
                    Console.WriteLine($"possybility for block \"{b}\" : \"{possibleTokenType?.Select(t => t.Code)?.Aggregate((a, b) => $"{a} {b}")}\"");
                tokenType = possibleTokenType?.Count > 0 ? possibleTokenType.First().Code : string.Empty;

                if (!string.IsNullOrWhiteSpace(tokenType))
                    foundToken.Add(new Token(tokenType, tokenValue, CurrentLine, nbColumn));
            }

            if (foundToken != null && foundToken.Count > 0 && Utils.debugMode)
            {
                Console.WriteLine($"token found : {foundToken.Select(t => t.Type).Aggregate((a, b) => $"{a} {b}")}\n");
                TokenBuffer.AddRange(foundToken);
            }

            return foundToken;
        }

        private string FormatLine(string fileLine) // TODO : fix composition of token bug (ex : / and //)
        {
            List<TokenType>? temp = Utils.tokenTypes.OrderByDescending(t => t.Order).ToList();

            foreach (var t in temp)
                fileLine = t.AddSpaceAround(fileLine);

            return fileLine;
        }
        public Token NextToken()
        {
            Last = Current;

            while (TokenBuffer.Count <= 0)
                Next();

            Current = TokenBuffer.First();
            TokenBuffer?.RemoveAt(0);

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
                Utils.PrintError("unrecognized_tokentype",false,type.Code);
        }
    }
}
