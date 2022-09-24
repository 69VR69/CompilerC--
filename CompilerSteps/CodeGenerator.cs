using System.Text;
using System.Text.RegularExpressions;

using CompilerC__.Objects;

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
        private static Dictionary<string, int> LabelCounter { get; set; }
        private static Dictionary<string, string> Labels { get; set; }

        public CodeGenerator()
        {
            SemanticScanner = new SemanticScanner();
            GeneratedCode = string.Empty;
            Labels = new()
            {
                {"break","lb" },
                {"loop","ll" },
                {"continue","lc" },
                {"if","li" },
                {"then","lt" },
                {"else","le" },
            };
            LabelCounter = new()
            {
                {"break",0 },
                {"loop",0 },
                {"continue",0 },
                {"if",0 },
                {"else",0 },
            };
        }

        public void GenerateCode()
        {
            Node node = SemanticScanner.SeS();
            if (Utils.debugMode)
                Console.WriteLine($"Tree node generated : \n{node}\n");

            Console.WriteLine("\n\nCode Generation start !\n");

            StringBuilder sb = new();
            sb.AppendLine(".start");
            sb.AppendLine($"resn {Utils.nbVar}");

            sb = GenerateNodeCode(node, sb);

            sb.AppendLine("\ndebug")
                .AppendLine("halt");

            GeneratedCode = sb.ToString();

            Console.WriteLine("\nCode Generation end !\n\n");

            if (Utils.debugMode)
                Console.WriteLine($"Generated assembly code :\n{GeneratedCode}");
            //CreateFileFromString();
        }

        private StringBuilder GenerateNodeCode(Node root, StringBuilder sb)
        {
            switch (root.Type) //block ?
            {
                case "declaration":
                case "seq":
                    break;

                case "const":
                    sb.AppendLine($"push {root.Value}");
                    break;

                case "ident":
                    sb.AppendLine($"get {root.Address}");
                    break;

                case "assign":
                    GenerateNodeCode(root.Childs[1], sb);
                    sb.AppendLine($"dup");
                    sb.AppendLine($"set {root.Childs[0].Address}");
                    break;

                ////////////////////////////////////////////////////////////////

                case "cond":
                    string ifLabel = GetNewLabel("if");
                    string elseLabel = GetNewLabel("else");

                    sb.AppendLine($"\t; start of cond n°{LabelCounter["if"]}");

                    GenerateNodeCode(root.Childs[0], sb); //condition

                    sb.AppendLine($"jumpf {elseLabel}");

                    GenerateNodeCode(root.Childs[1], sb); //then

                    sb.AppendLine($"jump {ifLabel}");
                    sb.AppendLine($".{elseLabel}");

                    if (root.Childs.Count == 3)
                        GenerateNodeCode(root.Childs[2], sb); //else

                    sb.AppendLine($".{ifLabel}");

                    sb.AppendLine($"\t; end of cond n°{LabelCounter["if"]}");

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

                ////////////////////////////////////////////////////////////////

                case "break":
                    sb.AppendLine($"jump {GetLabel("break")}");
                    break;

                case "loop":
                    string tempLblbBreak = GetNewLabel("break", post: true);
                    string looplabel = GetNewLabel("loop");

                    sb.AppendLine($"\t; start of loop n°{LabelCounter["loop"]}");

                    sb.AppendLine($".{looplabel}");

                    GenerateCodeForChilds(root, sb);

                    sb.AppendLine($"jump {looplabel} ");
                    sb.AppendLine($".lb{LabelCounter["break"]}");


                    sb.AppendLine($"\t; end of loop n°{LabelCounter["loop"]}");

                    SetLabelCounter(tempLblbBreak);
                    break;

                case "continue":
                    sb.AppendLine($"jump {GetLabel("continue")}");
                    break;

                case "continueLabel":
                    sb.AppendLine($".{GetNewLabel("continue")}");
                    break;

                ////////////////////////////////////////////////////////////////
                case "block":
                    GenerateCodeForChilds(root, sb);
                    break;

                default:
                    GenerateCodeForChilds(root, sb);
                    sb.AppendLine(root.Type);
                    break;

            }


            return sb;
        }

        private void CreateFileFromString()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "assembly_files");
            Directory.CreateDirectory(path);
            FileStream? f = File.Create(Path.Combine(path, $"{Path.GetRandomFileName()}.c"));

            byte[] data = new UTF8Encoding(true).GetBytes(GeneratedCode);
            f.Write(data, 0, data.Length);
            f.Close();

            Console.WriteLine($"File generated at : {f.Name}");
        }

        public void AddFileToLexical(List<string> fileLines)
        {
            LexicalScanner lexicalScanner = SemanticScanner.SyntaxScanner.LexicalScanner;
            lexicalScanner.FileLines = fileLines;
            lexicalScanner.NextToken();
        }

        private void GenerateCodeForChilds(Node root, StringBuilder sb)
        {
            foreach (Node child in root.Childs)
                GenerateNodeCode(child, sb);
        }

        private string GetLabel(string label)
        {
            return Labels[label] + LabelCounter[label];
        }
        private string GetNewLabel(string label, bool post = false)
        {
            return Labels[label] + (post ? LabelCounter[label]++ : ++LabelCounter[label]);
        }
        private void SetLabelCounter(string label)
        {
            LabelCounter[Labels.FirstOrDefault(x => x.Value == Regex.Match(label, @"\D+").Value).Key] = int.Parse(Regex.Match(label, @"\d+").Value);
        }
    }
}
