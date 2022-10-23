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

                    if (args.Contains("--simulator"))
                    {
                        Utils.withSimulator = true;
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

        #region Compilation Pipeline
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

                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine($"\nStart compiling {fileName}");

                CodeGenerator codeGenerator = new();
                codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(path));
                sb.AppendLine(codeGenerator.GenerateCode(fileName));

                Console.WriteLine($"End compiling {fileName}\n");

                if (Utils.withSimulator)
                {
                    LaunchMake(sb.ToString());
                    LaunchSimulator(fileName);
                }

                Console.WriteLine("----------------------------------------------------------------------------------------------------");
            }
        }

        public static void LaunchSimulator(string fileName)
        {

            string tempFile = Path.GetTempFileName();
            string simulatorPath = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "msm");
            string logFile = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log", $"{fileName}.txt");

            // Create a new process and redirect the output to the log file
            Process simulatorProcess = new()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(simulatorPath, "msm.exe"),
                    Arguments = $"-d {tempFile}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($" Execute simulator final command : {simulatorProcess.StartInfo.FileName} {simulatorProcess.StartInfo.Arguments}");

            // Start the process
            simulatorProcess.Start();
            simulatorProcess.WaitForExit();

            File.WriteAllText(logFile, simulatorProcess.StandardOutput.ReadToEnd() + simulatorProcess.StandardError.ReadToEnd());
            File.Delete(tempFile);

            // Print the log file path
            Console.WriteLine($"\nLog file: {logFile}");
        }

        public static void LaunchMake(string assemblyCode)
        {
            string tempFile = Path.GetTempFileName();
            string simulatorPath = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "msm");

            // Write the assembly code in the temporary file
            File.WriteAllText(tempFile, assemblyCode);

            // Create the directory log
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log"));

            string cygwinPath = "C:\\cygwin64\\bin\\bash";

            // Execute a make in the simulator directory through cygwin
            Process makeProcess = new()
            {
                StartInfo = new()
                {
                    FileName = cygwinPath,
                    Arguments = $" --login -c \"cd {simulatorPath.Replace('\\', '/')} && make -B\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };

            Console.WriteLine($" Execute make final command : {makeProcess.StartInfo.FileName} {makeProcess.StartInfo.Arguments}");

            makeProcess.Start();
            makeProcess.WaitForExit();

            if (makeProcess.ExitCode != 0)
            {
                Utils.PrintError("make_failed", false, makeProcess.StandardError.ReadToEnd());
                return;
            }
        }

        #endregion Compilation Pipeline
    }
}