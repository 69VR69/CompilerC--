using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.NewFolder
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

        public CodeGenerator(SemanticScanner semanticScanner)
        {
            SemanticScanner = semanticScanner;
            GeneratedCode = string.Empty;
        }

        public void GenerateCode()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(".start")
                .AppendLine("\thalt");

            GeneratedCode = sb.ToString();

            Console.WriteLine($"Generated assembly code :\n{GeneratedCode}");
            //CreateFileFromString();
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
    }
}
