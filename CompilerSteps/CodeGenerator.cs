using System.Text;

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

        private int NbLbl { get; set; }
        private int LblBreak { get; set; }

        public CodeGenerator()
        {
            SemanticScanner = new SemanticScanner();
            GeneratedCode = string.Empty;
            NbLbl = 0;
            LblBreak = NbLbl;
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

            sb.AppendLine("debug")
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
                case "const":
                    sb.AppendLine($"push {root.Value}");
                    break;

                case "ident":
                    sb.AppendLine($"get {root.Address}");
                    break;

                case "declaration":
                    break;

                case "assign":
                    GenerateNodeCode(root.Childs[1], sb);
                    sb.AppendLine($"dup");
                    sb.AppendLine($"set {root.Childs[0].Address}");
                    break;

                case "break":
                    sb.AppendLine($"jump l{LblBreak}");
                    break;

                case "loop":
                    int temp = LblBreak;
                    LblBreak = ++NbLbl;
                    int lbl_1 = ++NbLbl;

                    sb.AppendLine($".l{lbl_1}");

                    foreach (var c in root.Childs)
                        GenerateNodeCode(c, sb);

                    sb.AppendLine($"jump l{lbl_1} ");
                    sb.AppendLine($".l{LblBreak}");
                    LblBreak = temp;
                    break;

                case "continue":
                    int lbl_continue = ++NbLbl;
                    sb.AppendLine(".l" + lbl_continue);
                    break;

                default:
                    foreach (Node child in root.Childs)
                        sb = GenerateNodeCode(child, sb);

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
    }
}
