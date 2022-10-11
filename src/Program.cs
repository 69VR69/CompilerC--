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

                #region Runtime

                Console.WriteLine("\nStart runtime compilation");

                string runtimePath = Path.Combine(Directory.GetCurrentDirectory(), "runtime");
                string[] filepaths = Directory.GetFiles(runtimePath, "*.c", SearchOption.AllDirectories);

                string fileName;
                StringBuilder runtimeCode = new();

                foreach (string filepath in filepaths)
                {
                    fileName = Path.GetFileName(filepath);
                    Console.WriteLine($"\t\tStart compiling {fileName}");
                    codeGenerator = new();
                    codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(filepath));
                    runtimeCode.AppendLine(codeGenerator.GenerateCode());
                    Console.WriteLine($"\t\tEnd compiling {fileName}");
                }

                Console.WriteLine("\nEnd runtime compilation");

                #endregion Runtime

                #region Source

                Console.WriteLine("\nStart source compilation");

                fileName = Path.GetFileName(filePath);
                Console.WriteLine($"\t\tStart compiling {fileName}");
                codeGenerator = new();
                codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(filePath));
                codeGenerator.GenerateCode(runtimeCode.ToString());
                Console.WriteLine($"\t\tEnd compiling {fileName}");

                Console.WriteLine("\nEnd source compilation");

                #endregion Source

                #endregion Compile

            }
            catch (Exception e)
            {
                Utils.PrintError("unknow_error", false, e.ToString());
            }
        }
    }
}