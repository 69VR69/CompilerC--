using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public CodeGenerator()
        {
            SemanticScanner = new SemanticScanner();
            GeneratedCode = string.Empty;
        }

        public void GenerateCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".start");

            Node node = SemanticScanner.SeS();

            sb = GenerateNodeCode(node, sb);

            sb.AppendLine("\tdebug")
                .AppendLine("\thalt");

            GeneratedCode = sb.ToString();

            Console.WriteLine($"Generated assembly code :\n{GeneratedCode}");
            //CreateFileFromString();
        }

        private StringBuilder GenerateNodeCode(Node root, StringBuilder sb) // TODO : to complete with specials cases
        {
            if (root.Type == "const")
            {
                sb.AppendLine($"push {root.Value}");
                return sb;
            }

            foreach (Node child in root.Childs)
                sb = GenerateNodeCode(child, sb);

            sb.AppendLine(root.Type);

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
