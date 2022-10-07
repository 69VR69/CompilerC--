using CompilerC__.CompilerSteps;

namespace CompilerC__
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

                #region Load File

                Console.WriteLine("\nStart file loading");

                if (!File.Exists(filePath))
                {
                    Utils.PrintError("file_not_exist", false, filePath);
                    return;
                }

                List<string>? fileLines = null;
                try
                {
                    fileLines = File.ReadLines(filePath).ToList();
                }
                catch (System.Exception e)
                {
                    Utils.PrintError("file_read_error", false, e.Message);
                }

                if (fileLines == null || fileLines.Count == 0)
                {
                    Utils.PrintError("file_empty");
                }

                Console.WriteLine("File loading finished");

                #endregion Load File

                #region Compile

                Console.WriteLine("\nStart compilation");

                CodeGenerator codeGenerator =  new();
                codeGenerator.AddFileToLexical(fileLines);
                codeGenerator.GenerateCode();

                Console.WriteLine("\nCompilation finished");

                #endregion Compile

            }
            catch (System.Exception e)
            {
                Utils.PrintError("unknow_error", false, e.ToString());
            }
        }
    }
}