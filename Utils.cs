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
        #region Exception Management

        public static bool debugMode = false;
        public static List<CompilerException> exceptions = new List<CompilerException>
        {
            new CompilerException("unknow_error","An unknowed exception was trown, error message : {0}"),
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
                message = "Unknown exception raised";
            else if (args != null)
                for (int i = 0; i < args.Length; i++)
                    message = message.Replace($"{{{i}}}", args[i].ToString());

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
            new TokenType("eos", regex: "eos" ),
            new TokenType("const", regex: "\\d" ),
            new TokenType("ident", regex: "\\w" ), // TODO
            new TokenType("space", ' ','\t'),
            new TokenType("newLine", '\n','\r'),
            new TokenType("main", regex:"main"),
            new TokenType("preproc", '#'),
            new TokenType("comment", regex: "\\/\\/.*"),
            new TokenType("parenthesisIn", '('),
            new TokenType("parenthesisOut",')'),
            new TokenType("bracketIn", '{'),
            new TokenType("bracketOut", '}'),
            new TokenType("semicolon", ';'),
            new TokenType("plus", '+'),
            new TokenType("minus", '-'),
            new TokenType("star", '*'),
            new TokenType("slash", '/'),
            new TokenType("percent", '%'),
            new TokenType("ampersand",'&'),
            new TokenType("ampersandDouble",regex:"&&"),
            new TokenType("equal", '='),
            new TokenType("equalDouble", regex:"=="),
            new TokenType("comma", ','),
            new TokenType("exclamation",'!'),
            new TokenType("exclamationEqual", regex:"!="),
            new TokenType("lowChevron", '<'),
            new TokenType("lowChevronEqual", regex:"<="),
            new TokenType("upChevron",'>'),
            new TokenType("upChevronEqual",regex:">="),
            new TokenType("pipeDouble", regex:"\\|\\|"),
            new TokenType("return", regex: "return" ),
            new TokenType("int", regex: "int" ),
            new TokenType("if", regex: "if" ),
            new TokenType("else", regex: "else" ),
            new TokenType("for",regex: "for" ),
            new TokenType("while", regex: "while" ),
            new TokenType("do", regex: "do" ),
            new TokenType("break", regex: "break" ),
            new TokenType("continue",regex: "continue" ),
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
            new ("const"),
            new ("mult"),
            new ("div"),
            new ("mod"),
            new ("add"),
            new ("sub"),
            new ("less"),
            new ("lessequal"),
            new ("more"),
            new ("moreequal"),
            new ("equal"),
            new ("notequal"),
            new ("and"),
            new ("or"),
            new ("assign"),
            new ("loop"),
            new ("cond"),
            new ("seq"),
            new ("block"),
            new ("."),
            new ("."),
            new ("."),
            new ("."),
            new ("."),
            new ("."),
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

        #endregion Objects

        public static bool IsInDataTable(DataTable dt, string columnName, string value)
        {
            if (string.IsNullOrEmpty(columnName) || string.IsNullOrEmpty(value))
                return false;

            DataRow[]? drResult = dt.Select($"{columnName} = '{value}'");
            return drResult != null && drResult.Length > 0;
        }

        public static DataTable dtSymboles = new("Variables")
        {
            Columns = { { "id", typeof(string) }, { "type", typeof(string) }, { "adresse", typeof(byte[]) } },
        };
    }
}
