using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__
{
    internal static class Utils
    {
        #region Exception Management

        public static List<Exception> exceptions = new List<Exception>
        {
            new Exception("invalid_argument","You need to use a correct command syntax like : <programName> <fileName.c>"),
            new Exception("invalid_file_extension","Invalid file extension, file path provide : {0}"),
        };

        public static void printError(string exceptionCode, object? arg = null)
        {
            if (arg == null)
                printError(exceptionCode, null);
            else
                printError(exceptionCode, new object[] { arg });
        }

        public static void printError(string exceptionCode, object[]? args)
        {
            string message = exceptions.Where(e => e.Code == exceptionCode).First().Message;

            if (string.IsNullOrWhiteSpace(message))
            {
                message = "Unknown exception raised";
            }

            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    message = message.Replace($"{{{i}}}", args[i].ToString());
                }
            }

            Console.Error.WriteLine(message);

        }

        #endregion Exception Management

        #region Token Management

        public static Token currentToken, prevToken;
        public static List<Token> tokenBuffer = new List<Token>();
        public static List<TokenType> tokenTypes = new List<TokenType>
        {
            new TokenType("eos", regex: "eos" ),
            new TokenType("const", regex: "\\d" ),
            new TokenType("var", regex: "" ), // TODO
            new TokenType("space", ' ','\t'),
            new TokenType("newLine", '\n','\r'),
            new TokenType("preproc", '#'),
            new TokenType("comment", regex: "\\/\\/.*"),
            new TokenType("(", '('),
            new TokenType(")", ')'),
            new TokenType("{", '{'),
            new TokenType("}", '}'),
            new TokenType(";", ';'),
            new TokenType("return", regex: "return" ),
            new TokenType("int", regex: "int" ),
        };
        
        public static Token NextToken()
        {
            prevToken = currentToken;
            currentToken = tokenBuffer.First();
            tokenBuffer.RemoveAt(0);

            return currentToken;
        }
        
        public static void AddTokensToBuffer(List<Token> newTokens)
        {
            tokenBuffer.AddRange(newTokens);
        }

        #endregion Token Management
    }
}
