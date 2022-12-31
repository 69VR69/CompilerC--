using System.Text;
using System.Text.RegularExpressions;

using CompilerC__.Objects;
using CompilerC__.Objects.Types;
using CompilerC__.src;

namespace CompilerC__.CompilerSteps
{
    internal class CodeGenerator
    {
        public SemanticScanner SemanticScanner
        {
            get; set;
        }

        public string GeneratedCode
        {
            get; set;
        }
        private static Dictionary<string, int> LabelCounter { get; set; } = new()
            {
                {"break",0 },
                {"loop",0 },
                {"continue",0 },
                {"if",0 },
                {"else",0 },
            };
        private readonly static Dictionary<string, string> Labels = new()
            {
                {"break","lb" },
                {"loop","ll" },
                {"continue","lc" },
                {"if","li" },
                {"then","lt" },
                {"else","le" },
            };

        public CodeGenerator()
        {
            SemanticScanner = new SemanticScanner();
            GeneratedCode = string.Empty;
        }

        public string GenerateCode(string fileName)
        {
            if (Utils.debugMode)
                Console.WriteLine("\nCode Generation start !");

            StringBuilder sb = new();

            do
            {
                Node node = SemanticScanner.SeS();

                if (Utils.debugMode)
                    Console.WriteLine($"Tree node generated : \n{node}\n");

                sb = GenerateNodeCode(node, sb);

                if (GetLexicalScanner().Current.Type == Utils.GetNodeType("eos").Code)
                    break;

            } while (true);

            // if is runtime compiling, add it to the library
            if (Utils.runtimeMode == true)
                Utils.Libraries.First(l => l.Name == Utils.currentLibrary).SymbolTable = SemanticScanner.SymbolTable.First();

            GeneratedCode = sb.ToString();

            if (Utils.debugMode)
                Console.WriteLine("Code Generation end !\n");

            if (Utils.debugMode)
                Console.WriteLine($"Generated assembly code :\n{GeneratedCode}");

            CreateFileFromString(fileName);

            return GeneratedCode;
        }

        private StringBuilder GenerateNodeCode(Node root, StringBuilder sb)
        {
            switch (root.Type)
            {
                case "declaration":
                    break;

                case "lib":
                    sb.Append(Utils.GetLibrary(root.Value).AssemblyCode);
                    break;

                case "const":
                    sb.AppendLine($"push {root.Value}");
                    break;

                case "ident":
                    sb.AppendLine($"get {root.Address} ;variable {root.Value}");
                    break;

                case "indirection":
                    GenerateNodeCode(root.Childs[0], sb);
                    sb.AppendLine("read");
                    break;

                case "addrOf":
                    sb.AppendLine($"prep addrof");
                    sb.AppendLine($"push {Utils.nbVar}");
                    sb.AppendLine($"call 1");
                    break;

                case "semicolon":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("drop");
                    break;

                case "assign":
                    if (root.Childs[0].Type == "ident")
                    {
                        GenerateNodeCode(root.Childs[1], sb);
                        sb.AppendLine($"dup");
                        sb.AppendLine($"set {root.Childs[0].Address} ;variable {root.Childs[0].Value}");
                    }
                    else
                    {
                        GenerateNodeCode(root.Childs[1], sb);
                        sb.AppendLine("dup");
                        GenerateNodeCode(root.Childs[0].Childs[0], sb);
                        sb.AppendLine("write");
                    }
                    break;

                ////////////////////////////////////////////////////////////////

                case "cond":
                    string ifLabel = GetNewLabel("if");
                    string elseLabel = GetNewLabel("else");

                    sb.AppendLine($"\t; start of cond n°{GetLabelCounter(ifLabel)}");

                    GenerateNodeCode(root.Childs[0], sb); //condition

                    sb.AppendLine($"jumpf {elseLabel}");

                    GenerateNodeCode(root.Childs[1], sb); //then

                    sb.AppendLine($"jump {ifLabel}");
                    sb.AppendLine($".{elseLabel}");

                    if (root.Childs.Count == 3)
                        GenerateNodeCode(root.Childs[2], sb); //else

                    sb.AppendLine($".{ifLabel}");

                    sb.AppendLine($"\t; end of cond n°{GetLabelCounter(ifLabel)}");

                    break;

                case "more":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmpgt");
                    break;

                case "less":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmplt");
                    break;

                case "moreequal":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmpge");
                    break;

                case "lessequal":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmple");
                    break;

                case "equal":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmpeq");
                    break;

                case "notequal":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine("cmpne");
                    break;

                case "mult":
                    GenerateNodeCode(root.Childs[0], sb);
                    GenerateNodeCode(root.Childs[1], sb);
                    sb.AppendLine("mul");
                    break;

                ////////////////////////////////////////////////////////////////

                case "loop":
                    string tempLblbBreak = GetNewLabel("break");
                    string tempLblbContinue = GetNewLabel("continue");
                    string looplabel = GetNewLabel("loop");

                    sb.AppendLine($"\t; start of loop n°{GetLabelCounter(looplabel)}");

                    sb.AppendLine($".{looplabel}");

                    SetImbricatedLoop(root);
                    GenerateCodeForChilds(root, sb);

                    sb.AppendLine($"jump {looplabel} ");
                    sb.AppendLine($".lb{GetLabelCounter(tempLblbBreak)}");

                    if (root.isImbriquated())
                        sb.AppendLine($"\t; imbriquated => lb{GetLabelCounter(tempLblbBreak) - 1} && lc{GetLabelCounter(tempLblbContinue) - 1}");

                    sb.AppendLine($"\t; end of loop n°{GetLabelCounter(looplabel)}");

                    if (root.isImbriquated())
                    {
                        SetLabelCounter(tempLblbBreak);
                        SetLabelCounter(tempLblbContinue);
                    }
                    else if (root.Contains("loop", true))
                    {
                        SetLabelCounter(tempLblbBreak, 1);
                        SetLabelCounter(tempLblbContinue, 1);
                    }
                    break;

                case "break":
                    sb.AppendLine($"jump {GetLabel("break")}");
                    break;

                case "continue":
                    sb.AppendLine($"jump {GetLabel("continue")}");
                    break;

                case "continueLabel":
                    sb.AppendLine($".{GetLabel("continue")}");
                    break;

                ////////////////////////////////////////////////////////////////

                case "function":
                    string functionName = root.Value;
                    sb.AppendLine($"\n\t; start of function {functionName}");
                    sb.AppendLine($".{root.Value}");
                    sb.AppendLine($"resn {root.Address}");
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine($"push 0");
                    sb.AppendLine($"ret");
                    sb.AppendLine($"\t; end of function {functionName}\n");
                    break;

                case "call":
                    sb.AppendLine($"prep {root.Value}");
                    int nbParams = root.Childs.Where(x => x.Type != "semicolon").Count();
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine($"call {nbParams}");
                    break;

                case "return":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine($"ret");
                    break;

                ////////////////////////////////////////////////////////////////

                case "receive":
                    sb.AppendLine($"recv");
                    break;

                case "send":
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine($"send");
                    break;

                ////////////////////////////////////////////////////////////////
                case "block":
                case "seq":
                    GenerateCodeForChilds(root, sb);
                    break;

                default:
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine(root.Type);
                    break;
            }

            return sb;
        }

        private void CreateFileFromString(string fileName)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "assembly_files");
            Directory.CreateDirectory(path);
            FileStream? f = File.Create(Path.Combine(path, $"{fileName}.c"));

            byte[] data = new UTF8Encoding(true).GetBytes(GeneratedCode);
            f.Write(data, 0, data.Length);
            f.Close();

            Console.WriteLine($"File generated at : {f.Name}\n");
        }

        private LexicalScanner GetLexicalScanner()
        {
            return SemanticScanner.SyntaxScanner.LexicalScanner;
        }

        public void AddFileToLexical(List<string> fileLines)
        {
            LexicalScanner lexicalScanner = GetLexicalScanner();
            lexicalScanner.FileLines = fileLines;
            lexicalScanner.NextToken();
        }

        private void GenerateCodeForChilds(Node root, StringBuilder sb)
        {
            foreach (Node child in root.Childs)
                GenerateNodeCode(child, sb);
        }

        private static string GetLabel(string label)
        {
            return Labels[label] + LabelCounter[label];
        }
        private static string GetNewLabel(string label, bool post = false)
        {
            return Labels[label] + (post ? LabelCounter[label]++ : ++LabelCounter[label]);
        }
        private static int GetLabelCounter(string label)
        {
            return int.Parse(label.Substring(2));
        }
        private static void SetLabelCounter(string label, int modifier = -1)
        {
            string labelType = Labels.FirstOrDefault(x => x.Value == label.Substring(0, 2)).Key;
            int counter = int.Parse(label.Substring(2));
            LabelCounter[labelType] = counter + modifier;
        }

        private static void SetImbricatedLoop(Node root)
        {
            foreach (Node child in root.Childs)
            {
                if (child.Type == "loop")
                {
                    child.Address = 1;
                }
                SetImbricatedLoop(child);
            }
        }

        public static void AddFixedCode(StringBuilder sb)
        {
            sb.AppendLine("");

            sb.AppendLine(".start")
                .AppendLine("prep main")
                .AppendLine("call 0")
                .AppendLine("dbg")
                .AppendLine("halt");

            sb.AppendLine("");

            sb.AppendLine($@".addrof")
                .AppendLine("get -1")
                .AppendLine("get 0")
                .AppendLine("sub")
                .AppendLine("push 1")
                .AppendLine("sub")
                .AppendLine("ret");
        }
    }
}
