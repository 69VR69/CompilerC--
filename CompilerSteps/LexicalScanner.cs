using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private readonly TokenType[] tokenTypes = Utils.tokenTypes.Where(t => t.Code != "space" && t.Code != "comment" && t.Code != "preproc" && t.Code != "pipe").Select(t => t).ToArray();

        public LexicalScanner()
        {
            FileLines = null;
            CurrentLine = 0;
            TokenBuffer = new List<Token>();
            Current = new Token("eos", string.Empty);
            Last = Current;
        }

        private List<Token> Next()
        {
            List<Token>? foundToken = new();

            if (FileLines == null || CurrentLine >= FileLines?.Count)
            {
                foundToken = new List<Token> { new Token(Utils.tokenTypes.Where(t => t.Code == "eos").Select(t => t).ToArray()[0].Code, string.Empty) };
            }
            else
            {
                string fileLine = FileLines[CurrentLine];
                CurrentLine++;

                if (Utils.debugMode)
                    Console.WriteLine($"initial line : {fileLine}");

                // Reformat the line
                string formattedFileLine = FormatLine(fileLine);

                // Split the line in block of tokens
                List<string> blocks = formattedFileLine.Split(spacesDelemiter, StringSplitOptions.RemoveEmptyEntries).ToList();

                foreach (string b in blocks)
                {
                    // Stop if found a ignore character
                    if (ignored.Any(i => i.IsMatch(b)))
                        break;

                    // Recognize the token
                    List<TokenType> possibleTokenType = tokenTypes.Where(t => t.IsMatch(b)).OrderByDescending(t => t.Order).ToList();
                    if (possibleTokenType.Count > 1 && Utils.debugMode)
                        Console.WriteLine($"possybility for block \"{b}\" : \"{possibleTokenType?.Select(t => t.Code)?.Aggregate((a, b) => $"{a} {b}")}\"");
                    string tokenType = possibleTokenType?.Count > 0 ? possibleTokenType.First().Code : string.Empty;

                    string tokenValue = string.Empty;
                    switch (tokenType)
                    {
                        case "const":
                        case "ident":
                            tokenValue = b;
                            break;

                        default:
                            break;
                    }

                    if (!string.IsNullOrWhiteSpace(tokenType))
                        foundToken.Add(new Token(tokenType, tokenValue, CurrentLine));
                }
            }

            if (foundToken != null && foundToken.Count > 0)
            {
                MergeComposedTokens(foundToken);

                if (Utils.debugMode)
                    Console.WriteLine($"token found : {foundToken.Select(t => t.Type).Aggregate((a, b) => $"{a} {b}")}\n");

                TokenBuffer.AddRange(foundToken);
            }
            else
                return Next();

            return foundToken;
        }

        private string FormatLine(string fileLine)
        {
            List<TokenType>? tokenTypes = Utils.tokenTypes
                .Where(t => typeof(ComposedTokenType) != t.GetType())
                .OrderByDescending(t => t.Order)
                .ToList();

            foreach (var t in tokenTypes)
            {
                List<char>? matchedCharacters = t.MatchedCharacters;
                if (matchedCharacters?.Count > 0)
                    foreach (var m in matchedCharacters)
                        fileLine = fileLine.Replace(m.ToString(), $" {m} ");

                Regex? regex = t.Regex;
                if (regex != null)
                    foreach (var m in regex.Matches(fileLine))
                        fileLine = regex.Replace(fileLine, $" {m} ");
            }

            return fileLine;
        }
        private void MergeComposedTokens(List<Token> tokens)
        {
            List<TokenType>? composedTokens = Utils.tokenTypes
                .Where(t => typeof(ComposedTokenType) == t.GetType())
                .OrderByDescending(t => t.Order)
                .ToList();

            foreach (ComposedTokenType c in composedTokens.Cast<ComposedTokenType>())
            {
                List<TokenType> tokenList = c.TokenTypes;

                if (tokens.Count < tokenList.Count)
                    continue;

                int lastIndex = 0;
                while (lastIndex < tokens.Count)
                {
                    int startIndex = FindFirstMatch(tokens, tokenList, lastIndex);
                    if (CheckFollowMatchs(tokens, tokenList, startIndex, out lastIndex))
                    {
                        tokens.RemoveRange(startIndex, lastIndex - startIndex);
                        tokens.Insert(startIndex, new(c.Code, string.Empty, tokens[startIndex].Line));

                        if (ignored.Any(i => i.Code == c.Code))
                        {
                            tokens.RemoveRange(startIndex, tokens.Count);
                            break;
                        }
                    }
                    else
                        break;
                }
            }
        }

        private static bool CheckFollowMatchs(List<Token> tokens, List<TokenType> tokenList, int index, out int lastIndex)
        {
            bool isSame = true;
            lastIndex = 0;

            for (int i = 0; i < tokenList.Count; i++)
            {
                lastIndex = index + i;
                if (tokens[index + i].Type != tokenList[i].Code)
                {
                    isSame = false;
                    break;
                }
            }

            lastIndex++;

            return isSame;
        }
        private static int FindFirstMatch(List<Token> tokens, List<TokenType> tokenList, int startIndex = 0)
        {
            // find the first occurence of tokentypes in temptoken
            int index = 0;
            for (int i = startIndex; i < tokens.Count; i++)
            {
                if (tokens[i].Type == tokenList[0].Code)
                {
                    index = i;
                    break;
                }
            }

            return index;
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
                Utils.PrintError("unrecognized_tokentype", true, type.Code);
        }
    }
}
