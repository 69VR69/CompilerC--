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
        
        public static void printError(string exceptionCode, object[] args = null)
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
        public static List<TokenType> tokenTypes = new List<TokenType>
        {
            new TokenType("eos",new List<string>{ "EOF" }),
            new TokenType("const",new List<string>{ "???" }), // TODO : finish
        };

        #endregion Token Management
    }
}
