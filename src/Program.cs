using System.Diagnostics;
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

                if (!filePath.EndsWith(".c") && !Utils.testMode)
                {
                    Utils.PrintError("invalid_file_extension", false, filePath);
                    return;
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

                if (!Utils.testMode && filePath != ".")
                {
                    Console.WriteLine("\nStart source compilation");

                    LaunchCompilation(filePath, assemblyCode, false);

                    Console.WriteLine("End source compilation\n");
                }

                #endregion Source

                #region Test

                if (Utils.testMode && filePath == ".")
                {
                    Console.WriteLine("\nStart test compilation");

                    string testPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
                    Utils.debugMode = false;

                    LaunchCompilation(testPath, new(), true);

                    Console.WriteLine("End test compilation\n");
                }

                #endregion Test

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

        public static void LaunchSimulator(string fileName, string assemblyCode)
        {
            string tempFile = Path.GetTempFileName();
            string simulatorPath = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "msm");
            string logFile = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log", $"{fileName}.txt");

            // Write the assembly code in the temporary file
            File.WriteAllText(tempFile, assemblyCode);

            // Create the directory log
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log"));

            // Execute a make in the simulator directory
            Process makeProcess = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "make",
                    WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "simulator"),
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            makeProcess.Start();
            makeProcess.WaitForExit();

            if (makeProcess.ExitCode != 0)
            {
                Utils.PrintError("make_failed");
                return;
            }

            // Create a new process and redirect the output to the log file
            Process simulatorProcess = new()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = simulatorPath,
                    Arguments = $"-d \"{tempFile}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Start the process
            simulatorProcess.Start();
            File.WriteAllText(logFile, simulatorProcess.StandardOutput.ReadToEnd());
            File.Delete(tempFile);
            simulatorProcess.WaitForExit();

            // Print the log file path
            Console.WriteLine($"\nLog file: {logFile}");
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
                string fileName = Path.GetFileNameWithoutExtension(path);

                Console.WriteLine($"\nStart compiling {fileName}");

                CodeGenerator codeGenerator = new();
                codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(path));
                sb.AppendLine(codeGenerator.GenerateCode(fileName));

                Console.WriteLine($"End compiling {fileName}\n");

                if (Utils.debugMode)
                {
                    LaunchSimulator(fileName, sb.ToString());
                }

            }
        }
    }
}