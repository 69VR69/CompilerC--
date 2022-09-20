using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CompilerC__.Objects;
using CompilerC__.Objects.Types;

namespace CompilerC__
{
    internal static class Utils
    {
        public static int nbVar = 0;

        #region Exception Management

        public static bool debugMode = false;
        public static List<CompilerException> exceptions = new List<CompilerException>
        {
            new CompilerException("unknow_error","An exception was trown, error message : {0}"),
            new CompilerException("invalid_argument","You need to use a correct command syntax like : programName fileName.c <--debug>"),
            new CompilerException("invalid_file_extension","Invalid file extension, file path provide : {0}"),
            new CompilerException("file_not_exist","File doesn't exist, file path provide : {0}"),
            new CompilerException("file_read_error","A file read error occurs, error message : {0}"),
            new CompilerException("file_empty","The file is empty, file path provide : {0}"),
            new CompilerException("unrecognized_element","Unrecognized element '{0}'"),
            new CompilerException("unrecognized_grammargroup","Unrecognized grammar group '{0}'"),
            new CompilerException("unrecognized_tokentype","Unrecognized token type '{0}'"),
            new CompilerException("no_element_group","No element group provide for grammar group '{0}'"),
            new CompilerException("unrecognized_token","Unrecognized token '{0}' at ({1},{2})"),
            new CompilerException("symbol_already_declared","A symbol with the identification code '{0}' is already declared in this scope"),
            new CompilerException("assign_to_non_var","Assignation to {0} is impossible as it's not a variable"),
            new CompilerException("unrecognized_symbol","Unrecognized symbol with the identification code '{0}' "),
        };

        public static void PrintError(string exceptionCode, bool isBlocking = false, object? arg = null)
        {
            if (arg == null)
                PrintError(exceptionCode, isBlocking, null);
            else
                PrintError(exceptionCode, isBlocking, new object[] { arg });
        }

        public static void PrintError(string exceptionCode, bool isBlocking = false, params object[]? args)
        {
            // Form the error message
            CompilerException? ex = exceptions.Where(e => e.Code == exceptionCode).FirstOrDefault();
            string? message = ex?.Message;
            if (string.IsNullOrWhiteSpace(message))
            {
                if (string.IsNullOrEmpty(exceptionCode))
                    message = "Unknown exception raised";
                else
                    message = $"Message missing for thrown exception code : {exceptionCode}";
            }
            else if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                    message = message.Replace($"{{{i}}}", args[i].ToString());
            }

            // Print or raise exception
            if (isBlocking)
                throw new System.Exception(message);

            Console.Error.WriteLine(message);

        }

        #endregion Exception Management

        #region Objects

        #region Token Management

        public static List<TokenType> tokenTypes = new()
        {
            new TokenType("eos",0, regex: "eos" ),
            new TokenType("const", 0,regex: "\\d+" ),
            new TokenType("ident", 5,regex: "[a-zA-Z]+" ),
            new TokenType("space",0, ' ','\t'),
            new TokenType("newLine", 0,'\n','\r'),
            new TokenType("main",10, regex:"main"),
            new TokenType("preproc",0, '#'),
            new TokenType("comment", 5,regex: "\\/\\/.*"),
            new TokenType("parenthesisIn",0, '('),
            new TokenType("parenthesisOut",0,')'),
            new TokenType("bracketIn",0, '{'),
            new TokenType("bracketOut",0, '}'),
            new TokenType("semicolon",0, ';'),
            new TokenType("plus",0, '+'),
            new TokenType("minus",0, '-'),
            new TokenType("star",0, '*'),
            new TokenType("slash",0, '/'),
            new TokenType("percent",0, '%'),
            new TokenType("ampersand",0,'&'),
            new TokenType("ampersandDouble",5,regex:"&&"),
            new TokenType("equal", 0,'='),
            new TokenType("equalDouble", 5,regex:"=="),
            new TokenType("comma", 0,','),
            new TokenType("exclamation",0,'!'),
            new TokenType("exclamationEqual",5, regex:"!="),
            new TokenType("lowChevron", 0,'<'),
            new TokenType("lowChevronEqual",5, regex:"<="),
            new TokenType("upChevron",0,'>'),
            new TokenType("upChevronEqual",5,regex:">="),
            new TokenType("pipeDouble",5, regex:"\\|\\|"),
            new TokenType("return", 10,regex: "return" ),
            new TokenType("int", 10,regex: "int" ),
            new TokenType("if", 10,regex: "if" ),
            new TokenType("else",10, regex: "else" ),
            new TokenType("for",10,regex: "for" ),
            new TokenType("while",10, regex: "while" ),
            new TokenType("do", 10,regex: "do" ),
            new TokenType("break",10, regex: "break" ),
            new TokenType("continue",10,regex: "continue" ),
        };

        public static TokenType? GetTokenType(string code)
        {
            return tokenTypes.Find(t => t.Code == code);
        }
        public static TokenType[]? GetTokenType(params string[] code)
        {
            return tokenTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();
        }

        #endregion Token Management

        #region Nodes

        public static List<NodeType> nodeTypes = new()
        {
            new ("var"),
            new ("const"),
            new ("mult"),
            new ("div"),
            new ("mod"),
            new ("add"),
            new ("sub"),
            new ("not"),
            new ("less"),
            new ("lessequal"),
            new ("more"),
            new ("moreequal"),
            new ("equal"),
            new ("notequal"),
            new ("and"),
            new ("or"),
            new ("assign"),
            new ("break"),
            new ("continue"),
            new ("loop"),
            new ("cond"),
            new ("seq"),
            new ("declaration"),
            new ("block"),
        };

        public static NodeType? GetNodeType(string code)
        {
            return nodeTypes.Find(t => t.Code == code);
        }
        public static NodeType[]? GetNodeType(params string[] code)
        {
            return nodeTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();
        }

        #endregion Nodes

        #region Operators
        public static List<Operation> operations = new()
        {
            new (GetTokenType("star"), 6, true, GetNodeType("mult")),
            new (GetTokenType("slash"), 6, true, GetNodeType("div")),
            new (GetTokenType("percent"), 6, true, GetNodeType("mod")),
            new (GetTokenType("plus"), 5, true, GetNodeType("add")),
            new (GetTokenType("minus"), 5, true, GetNodeType("sub")),
            new (GetTokenType("lowChevron"), 4, true, GetNodeType("less") ),
            new (GetTokenType("lowChevronEqual"), 4, true, GetNodeType("lessequal")),
            new (GetTokenType("upChevron"), 4, true, GetNodeType("more")),
            new (GetTokenType("upChevronEqual"), 4, true, GetNodeType("moreequal")),
            new (GetTokenType("equalDouble"), 4, true, GetNodeType("equal")),
            new (GetTokenType("exclamationEqual"), 4, true, GetNodeType("notequal")),
            new (GetTokenType("ampersandDouble"), 3, true, GetNodeType("and")),
            new (GetTokenType("pipeDouble"), 2, true, GetNodeType("or")),
            new (GetTokenType("equal"), 1, false, GetNodeType("assign")),
        };

        public static Operation? GetOperation(string tokenType)
        {
            if (tokenType == null)
                return null;
            else
                return operations.Find(t => t.TokenType.Code == tokenType);
        }
        public static Operation[]? GetOperation(params string[] tokenType)
        {
            if (tokenType == null)
                return null;
            else
                return operations.Where(t => tokenType.Contains(t.TokenType.Code)).OrderBy(t => t.Priority).Select(t => t).ToArray();
        }

        #endregion Operators

        #region Symbols
        public static List<SymbolType> symbolTypes = new()
        {
            new ("var"),
            new ("func"),
        };

        public static SymbolType? GetSymbolType(string symbolType)
        {
            if (symbolType == null)
                return null;
            else
                return symbolTypes.Find(t => t.Code == symbolType);
        }
        public static SymbolType[]? GetSymbolTypen(params string[] symbolType)
        {
            if (symbolType == null)
                return null;
            else
                return symbolTypes.Where(t => symbolType.Contains(t.Code)).Select(t => t).ToArray();
        }

        #endregion Operators

        #endregion Objects
    }
}
