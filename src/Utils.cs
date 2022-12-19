using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CompilerC__.Objects;
using CompilerC__.Objects.Types;
using CompilerC__.src.Objects;

namespace CompilerC__.src
{
    internal static class Utils
    {
        public static int nbVar = 0;
        public static bool isEOS = false;

        #region Exception Management

        public static bool testMode = false;
        public static bool debugMode = false;
        public static bool withSimulator = false;
        public static List<CompilerException> exceptions = new()
        {
            new CompilerException("unknow_error","An exception was trown, error message : {0}"),
            new CompilerException("make_failed","The make of the simulator failed with exception :\n{0}"),
            new CompilerException("simulator_failed","The execution of the simulator failed with exception :\n{0}"),
            new CompilerException("invalid_argument","You need to use a correct command syntax like : programName fileName.c <--debug> or programName . <--test> for unit tests"),
            
            // File exceptions
            new CompilerException("invalid_file_extension","Invalid file extension, file path provide : {0}"),
            new CompilerException("file_not_exist","File doesn't exist, file path provide : {0}"),
            new CompilerException("file_read_error","A file read error occurs, error message : {0}"),
            new CompilerException("file_empty","The file is empty, file path provide : {0}"),
            
            // Lexical or Syntax exceptions
            new CompilerException("unrecognized_tokentype","One or more unrecognized token type: '{0}'"),
            new CompilerException("unrecognized_nodetype","One or more unrecognized node type: '{0}'"),
            new CompilerException("unrecognized_nodetype","One or more unrecognized node type: '{0}'"),
            new CompilerException("unrecognized_symboltype","One or more unrecognized symbol type: '{0}'"),
            new CompilerException("unrecognized_operation","One or more unrecognized operation: '{0}'"),
            new CompilerException("unrecognized_token","Unrecognized token '{0}' at line {1}"),
            new CompilerException("function_declaration_missing","Function declaration missing at line {0}"),
            
            // Semantic exceptions
            new CompilerException("unrecognized_symbol","Unrecognized symbol with the identification code '{0}' "),
            new CompilerException("library_not_found","Library '{0}' not found"),
            new CompilerException("symbol_already_declared","A symbol with the identification code '{0}' is already declared in this scope"),
            new CompilerException("assign_to_non_var","Assignation to '{0}' is impossible as it's not a variable at line {1}"),
            new CompilerException("function_already_exist","Function '{0}' already declared"),
            new CompilerException("var_without_ident","Cannot use a variable '{0}' without value at line {1}"),
            new CompilerException("function_not_found","Function '{0}' not found"),
            new CompilerException("wrong_number_of_parameters","Wrong number of parameters provide to function named '{0}'"),
            new CompilerException("addrof_not_on_ident","Get the address of the non-indent '{0}' is impossible  at line {1}"),
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
                throw new Exception($"\n{message}");

            Console.Error.WriteLine($"\n{message}");

        }

        #endregion Exception Management

        #region Objects

        #region Token

        public static List<TokenType> tokenTypes = new()
        {
            new TokenType("eos",0, regex: "eos" ),
            new TokenType("const", 0,regex: "\\d+" ),
            new TokenType("ident", 5,regex: "[a-zA-Z](\\w|\\.)*" ),
            new TokenType("space",0, ' ','\t'),
            new TokenType("newLine", 0,'\n','\r'),
            new TokenType("preproc",0, '#'),
            new TokenType("parenthesisIn",0, '('),
            new TokenType("parenthesisOut",0,')'),
            new TokenType("bracketIn",0, '{'),
            new TokenType("bracketOut",0, '}'),
            new TokenType("squareBracketIn",0, '['),
            new TokenType("squareBracketOut",0, ']'),
            new TokenType("semicolon",0, ';'),
            new TokenType("plus",0, '+'),
            new TokenType("minus",0, '-'),
            new TokenType("star",0, '*'),
            new TokenType("slash",0, '/'),
            new TokenType("percent",0, '%'),
            new TokenType("ampersand",0,'&'),
            new TokenType("pipe",0,'|'),
            new TokenType("equal", 0,'='),
            new TokenType("comma", 0,','),
            new TokenType("exclamation",0,'!'),
            new TokenType("lowChevron", 0,'<'),
            new TokenType("upChevron",0,'>'),
            new TokenType("return", 10,regex: "return" ),
            new TokenType("int", 10,regex: "int" ),
            new TokenType("if", 10,regex: "if" ),
            new TokenType("else",10, regex: "else" ),
            new TokenType("for",10,regex: "for" ),
            new TokenType("while",10, regex: "while" ),
            new TokenType("do", 10,regex: "do" ),
            new TokenType("break",10, regex: "break" ),
            new TokenType("continue",10,regex: "continue" ),
            new TokenType("send",10,regex: "send" ),
            new TokenType("receive",10,regex: "recv" ),
        };
        public static void AddComposedTokenTypes()
        {
            tokenTypes.AddRange(new List<TokenType>()
            {
                new ComposedTokenType("comment", 5, GetTokenType("slash"), GetTokenType("slash")),
                new ComposedTokenType("ampersandDouble", 5, GetTokenType("ampersand"), GetTokenType("ampersand")),
                new ComposedTokenType("equalDouble", 5, GetTokenType("equal"), GetTokenType("equal")),
                new ComposedTokenType("exclamationEqual", 5, GetTokenType("exclamation"), GetTokenType("equal")),
                new ComposedTokenType("lowChevronEqual", 5, GetTokenType("lowChevron"), GetTokenType("equal")),
                new ComposedTokenType("upChevronEqual", 5, GetTokenType("upChevron"), GetTokenType("equal")),
                new ComposedTokenType("pipeDouble", 5, GetTokenType("pipe"), GetTokenType("pipe")),
            });
        }

        public static TokenType GetTokenType(string code)
        {
            TokenType? t = tokenTypes.Find(t => t.Code == code);

            if (t == null)
                PrintError("unrecognized_tokentype", true, code);

            return t;
        }
        public static TokenType[] GetTokenType(params string[] code)
        {
            TokenType[]? t = tokenTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();

            if (t == null)
                PrintError("unrecognized_tokentype", true, code);

            return t;
        }

        #endregion Token

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
            new ("indirection"),
            new ("addrOf"),
            new ("send"),
            new ("receive"),
            new ("break"),
            new ("continue"),
            new ("continueLabel"),
            new ("loop"),
            new ("cond"),
            new ("seq"),
            new ("declaration"),
            new ("block"),
            new ("lib"),
            new ("eos"),
        };

        public static NodeType GetNodeType(string code)
        {
            NodeType? n = nodeTypes.Find(t => t.Code == code);

            if (n == null)
                PrintError("unrecognized_nodetype", true, code);

            return n;
        }
        public static NodeType[] GetNodeType(params string[] code)
        {
            NodeType[]? n = nodeTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();

            if (n == null)
                PrintError("unrecognized_nodetype", true, code);

            return n;
        }

        #endregion Nodes

        #region Operation
        public static List<Operation> operations = new();

        public static void AddOperation()
        {
            operations.AddRange(new List<Operation>()
            {
                new(GetTokenType("star"), 6, true, GetNodeType("mult")),
                new(GetTokenType("slash"), 6, true, GetNodeType("div")),
                new(GetTokenType("percent"), 6, true, GetNodeType("mod")),
                new(GetTokenType("plus"), 5, true, GetNodeType("add")),
                new(GetTokenType("minus"), 5, true, GetNodeType("sub")),
                new(GetTokenType("lowChevron"), 4, true, GetNodeType("less")),
                new(GetTokenType("lowChevronEqual"), 4, true, GetNodeType("lessequal")),
                new(GetTokenType("upChevron"), 4, true, GetNodeType("more")),
                new(GetTokenType("upChevronEqual"), 4, true, GetNodeType("moreequal")),
                new(GetTokenType("equalDouble"), 4, true, GetNodeType("equal")),
                new(GetTokenType("exclamationEqual"), 4, true, GetNodeType("notequal")),
                new(GetTokenType("ampersandDouble"), 3, true, GetNodeType("and")),
                new(GetTokenType("pipeDouble"), 2, true, GetNodeType("or")),
                new(GetTokenType("equal"), 1, false, GetNodeType("assign")),
            });
        }

        public static bool IsOperation(string tokenType)
        {
            return operations.Exists(o => o.TokenType.Code == tokenType);
        }

        public static Operation? GetOperation(string tokenType)
        {
            Operation? o = operations.Find(o => o.TokenType.Code == tokenType);

            return o;
        }
        public static Operation[]? GetOperation(params string[] tokenType)
        {
            Operation[]? o = operations.Where(o => tokenType.Contains(o.TokenType.Code)).OrderBy(o => o.Priority).Select(o => o).ToArray();

            return o;
        }

        #endregion Operation

        #region Symbols
        public static List<SymbolType> symbolTypes = new()
        {
            new ("var"),
            new ("func"),
        };

        public static SymbolType GetSymbolType(string symbolType)
        {
            SymbolType? s = symbolTypes.Find(t => t.Code == symbolType);

            if (s == null)
                PrintError("unrecognized_symboltype", true, symbolType);

            return s;
        }
        public static SymbolType[] GetSymbolType(params string[] symbolType)
        {

            SymbolType[]? s = symbolTypes.Where(t => symbolType.Contains(t.Code)).Select(t => t).ToArray();

            if (s == null)
                PrintError("unrecognized_symboltype", true, symbolType);

            return s;
        }

        #endregion Operators

        #region Library

        public static List<Library> Libraries = new();
        public static string currentLibrary = string.Empty;
        public static bool runtimeMode = false;

        public static Library GetLibrary(string libraryName)
        {
            Library? l = Libraries.Find(l => l.Name == libraryName);

            if (l == null)
                PrintError("unrecognized_library", true, libraryName);

            return l;
        }

        #endregion Library

        #endregion Objects

        #region Methods

        public static List<string> LoadFileFromPath(string path)
        {
            if (Utils.debugMode)
                Console.WriteLine("\nStart loading file");

            if (!File.Exists(path))
            {
                Utils.PrintError("file_not_exist", false, path);
                return new List<string>();
            }

            List<string>? fileLines = null;
            try
            {
                fileLines = File.ReadLines(path).ToList();
            }
            catch (Exception e)
            {
                Utils.PrintError("file_read_error", false, e.Message);
            }

            if (fileLines == null || fileLines.Count == 0)
            {
                Utils.PrintError("file_empty");
            }

            if (Utils.debugMode)
                Console.WriteLine("End loading file\n");

            return fileLines;
        }

        #endregion Methods
    }
}
