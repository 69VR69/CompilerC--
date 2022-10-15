using System.Text;

using CompilerC__.CompilerSteps;

namespace CompilerC__.src
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Init Utils
            Utils.AddComposedTokenTypes();
            Utils.AddOperation();

            try
            {
                #region Parse arguments

                if (args.Length == 0)
                {
                    Utils.PrintError("invalid_argument");
                    return;
                }

                string filePath = args[0];
                if (!filePath.EndsWith(".c"))
                {
                    Utils.PrintError("invalid_file_extension", false, filePath);
                    return;
                }

                if (args.Length > 1)
                {
                    if (args.Contains("--debug"))
                    {
                        Utils.debugMode = true;
                    }

                    if (args.Contains("--test"))
                    {
                        Utils.testMode = true;
                    }
                }

                #endregion Parse arguments

                #region Compile
                CodeGenerator codeGenerator = new();
                StringBuilder assemblyCode = new();

                #region Runtime

                Console.WriteLine("\nStart runtime compilation");
                
                string runtimePath = Path.Combine(Directory.GetCurrentDirectory(), "runtime");

                LaunchCompilation(runtimePath, assemblyCode, true);

                Console.WriteLine("End runtime compilation\n");

                #endregion Runtime

                #region Source

                Console.WriteLine("\nStart source compilation");

                LaunchCompilation(filePath, assemblyCode, false);

                Console.WriteLine("End source compilation\n");

                #endregion Source

                #endregion Compile

                #region Print assembly code

                Console.WriteLine("\nAssembly code:");
                Console.WriteLine(assemblyCode);

                #endregion Print assembly code

            }
            catch (Exception e)
            {
                Utils.PrintError("unknow_error", false, e.ToString());
            }
        }

        public static void LaunchCompilation(string path, StringBuilder sb, bool isDirectory = false)
        {
            if (isDirectory)
            {
                string[] filepaths = Directory.GetFiles(path, "*.c", SearchOption.AllDirectories);

                foreach (string filepath in filepaths)
                {
                    LaunchCompilation(filepath, sb, false);
                }
            }
            else
            {
                string fileName = Path.GetFileName(path);
                Console.WriteLine($"\nStart compiling {fileName}");
                CodeGenerator codeGenerator = new();
                codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(path));
                sb.AppendLine(codeGenerator.GenerateCode());
                Console.WriteLine($"End compiling {fileName}\n");
            }
        }
    }
}