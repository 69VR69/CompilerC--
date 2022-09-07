using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CompilerC__.Objects;

namespace CompilerC__
{
    internal static class Utils
    {
        #region Exception Management

        public static bool debugMode = false;
        public static List<Exception> exceptions = new List<Exception>
        {
            new Exception("unknow_error","An unknowed exception was trown, error message : {0}"),
            new Exception("invalid_argument","You need to use a correct command syntax like : programName fileName.c <--debug>"),
            new Exception("invalid_file_extension","Invalid file extension, file path provide : {0}"),
            new Exception("file_not_exist","File doesn't exist, file path provide : {0}"),
            new Exception("file_read_error","A file read error occurs, error message : {0}"),
            new Exception("file_empty","The file is empty, file path provide : {0}"),
            new Exception("unrecognized_element","Unrecognized element '{0}'"),
            new Exception("unrecognized_grammargroup","Unrecognized grammar group '{0}'"),
            new Exception("unrecognized_tokentype","Unrecognized token type '{0}'"),
            new Exception("no_element_group","No element group provide for grammar group '{0}'"),
            new Exception("unrecognized_token","Unrecognized token '{0}' at ({1},{2})"),
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
            Exception? ex = exceptions.Where(e => e.Code == exceptionCode).FirstOrDefault();
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

        #region Token Management

        public static List<TokenType> tokenTypes = new()
        {
            new TokenType("eos", regex: "eos" ),
            new TokenType("const", regex: "\\d" ),
            new TokenType("var", regex: "" ), // TODO
            new TokenType("space", ' ','\t'),
            new TokenType("newLine", '\n','\r'),
            new TokenType("main", regex:"main"),
            new TokenType("preproc", '#'),
            new TokenType("comment", regex: "\\/\\/.*"),
            new TokenType("(", '('),
            new TokenType(")",')'),
            new TokenType("{", '{'),
            new TokenType("}", '}'),
            new TokenType(";", ';'),
            new TokenType("+", '+'),
            new TokenType(code: "-", '-'),
            new TokenType("*", '*'),
            new TokenType("/", '/'),
            new TokenType("%", '%'),
            new TokenType("&",'&'),
            new TokenType("&&",regex:"&&"),
            new TokenType("=", '='),
            new TokenType("==", regex:"=="),
            new TokenType(",", ','),
            new TokenType("!",'!'),
            new TokenType("!=", regex:"!="),
            new TokenType("<", '<'),
            new TokenType("<=", regex:"<="),
            new TokenType(">",'>'),
            new TokenType(">=",regex:">="),
            new TokenType("||", regex:"\\|\\|"),
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

        public static List<GrammarGroup> grammarGroups = new()
        {
            new GrammarGroup("General",'G'),
            new GrammarGroup("Function",'F'),
            new GrammarGroup("Instrustion",'I'),
            new GrammarGroup("Expression",'E'),
            new GrammarGroup("Prefix",'P'),
            new GrammarGroup("Sufix",'S'),
            new GrammarGroup("Atome",'A'),
        };

        public static List<NodeType> nodeTypes = new()
        {
            new NodeType("const",""),
        };

        public static List<ElementGroup> elementGroups = new()
        {
            #region General

            new ElementGroup(GetGroup("General"),0,
                new Element[]{GetGroup("Function") },
                new ()),
            
            #endregion General

            #region Function

            new ElementGroup(GetGroup("Function"),0,
                new Element[]{GetGroup("Instrustion") },
                new ()),
            
            #endregion Function

            #region Instruction

            new ElementGroup(GetGroup("Instruction"),0,
                new Element[]{GetGroup("Expression") },
                new ()),
            
            #endregion Instruction

            #region Expression

            new ElementGroup(GetGroup("Expression"),0,
                new Element[]{GetGroup("Prefix") },
                new ()),
            
            #endregion Expression

            #region Prefix

            new (GetGroup("Prefix"),0,
                new Element[]{GetGroup("Sufix") },
                new ()),

            new (GetGroup("Prefix"),1,
                new Element[]{GetTokenType("-"),GetGroup("Prefix") },
                new ()),

            new (GetGroup("Prefix"),2,
                new Element[]{GetTokenType("+"),GetGroup("Prefix") },
                new("neg_a")),
            
            #endregion Prefix
            
            #region Sufix

            new ElementGroup(GetGroup("Sufix"),0,
                new Element[]{GetGroup("Atome") },
                new ()),
            
            #endregion Sufix
           
            #region Atome

            new (GetGroup("Atome"),0,
                new Element[]{GetTokenType("const") },
                new (""))
            
            #endregion Atome
        };

        #region Methods

        public static TokenType? GetTokenType(string code)
        {
            return tokenTypes.Find(t => t.Code == code);
        }
        public static TokenType[]? GetTokenType(params string[] code)
        {
            return tokenTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();
        }
        public static NodeType? GetNodeType(string code)
        {
            return nodeTypes.Find(t => t.Code == code);
        }
        public static NodeType[]? GetNodeType(params string[] code)
        {
            return nodeTypes.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();
        }
        public static GrammarGroup? GetGroup(string code)
        {
            return grammarGroups.Find(g => g.Code == code);
        }
        public static GrammarGroup? GetGroup(GrammarGroup grammarGroup)
        {
            return grammarGroups.Find(g => g == grammarGroup);
        }
        public static GrammarGroup[]? GetGroup(params string[] code)
        {
            return grammarGroups.Where(t => code.Contains(t.Code)).Select(t => t).ToArray();
        }
        public static ElementGroup[]? GetElementGroups(GrammarGroup grammarGroup)
        {
            return elementGroups.Where(g => g.Category == grammarGroup).OrderByDescending(g => g.Order).Select(g => g).ToArray();
        }

        #endregion Methods

        #endregion Token Management

        public static DataTable dtOperations = new("Operations")
        {
            Columns = { { "token", typeof(string) }, { "prio", typeof(int) }, { "isLeftAsso", typeof(bool) }, { "node", typeof(NodeType) } },
            Rows = {
                { "*", 6, true, GetNodeType("mult") },
                { "/", 6, true, GetNodeType("div") },
                { "+", 5, true, GetNodeType("add") },
                { "-", 5, true, GetNodeType("sub") },
                { "<", 4, true, GetNodeType("less") },
                { "<=", 4, true, GetNodeType("lessequal") },
                { ">", 4, true, GetNodeType("more") },
                { ">=", 4, true, GetNodeType("moreequal") },
                { "==", 4, true, GetNodeType("equal") },
                { "!=", 4, true, GetNodeType("notequal") },
                { "&&", 3, true, GetNodeType("and") },
                { "||", 2, true, GetNodeType("or") },
                { "=", 1, false, GetNodeType("assign") },
            }
        };
    }
}
