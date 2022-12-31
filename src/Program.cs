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
                StringBuilder runtimeCode = new();
                Dictionary<string, StringBuilder> assemblyCodes = new();

                #region Runtime

                Console.WriteLine("\nStart runtime compilation");

                string runtimePath = Path.Combine(Directory.GetCurrentDirectory(), "runtime");

                Utils.runtimeMode = true;

                LaunchCompilation(runtimePath, runtimeCode, codeGenerator, true);

                var test = Utils.Libraries;

                Utils.runtimeMode = false;

                Console.WriteLine("End runtime compilation\n");

                #endregion Runtime

                CodeGenerator.AddFixedCode(runtimeCode);

                #region Source

                if (!Utils.testMode && filePath != ".")
                {
                    Console.WriteLine("\nStart source compilation");

                    foreach (KeyValuePair<string, StringBuilder> assembly_code in LaunchCompilation(filePath, new StringBuilder(), codeGenerator, false))
                    {
                        assemblyCodes.Add(assembly_code.Key, assembly_code.Value);

                    }

                    Console.WriteLine("End source compilation\n");
                }

                #endregion Source

                #region Test

                if (Utils.testMode && filePath == ".")
                {
                    Console.WriteLine("\nStart test compilation");

                    string testPath = Path.Combine(Directory.GetCurrentDirectory(), "test");
                    Utils.debugMode = false;

                    foreach (var t in LaunchCompilation(testPath, new StringBuilder(), codeGenerator, true))
                    {
                        assemblyCodes.Add(t.Key, t.Value);
                    }

                    Console.WriteLine("End test compilation\n");
                }

                #endregion Test

                #region Merge assemblyCode
                Dictionary<string, StringBuilder> temp = new();

                foreach (KeyValuePair<string, StringBuilder> assemblyCode in assemblyCodes)
                {
                    StringBuilder runtimeTemp = new(runtimeCode.ToString());
                    string key = assemblyCode.Key;
                    string value = assemblyCode.Value.ToString();

                    temp.Add(key, new(runtimeTemp.AppendLine(value).ToString()));
                }

                assemblyCodes = temp;

                #endregion Merge assemblyCode

                #endregion Compile

                #region Print assembly code

                if (Utils.debugMode)
                {
                    foreach (KeyValuePair<string, StringBuilder> assemblyCode in assemblyCodes)
                    {
                        Console.WriteLine($"\n{assemblyCode.Key}'s assembly code:\n");
                        Console.WriteLine(assemblyCode.Value);
                    }
                }

                #endregion Print assembly code

                #region Execute with simulator

                if (Utils.withSimulator)
                {
                    LaunchMake();

                    List<string> fileWithError = new();
                    foreach (KeyValuePair<string, StringBuilder> assemblyCode in assemblyCodes)
                    {
                        Console.WriteLine($"\n{assemblyCode.Key}'s execution:");
                        int exitCode = LaunchSimulator(assemblyCode);
                        Console.WriteLine("----------------------------------------------------------------------------------------------------");

                        if (exitCode != 0)
                        {
                            fileWithError.Add(assemblyCode.Key);
                        }
                    }

                    if (fileWithError.Count > 0)
                    {
                        Console.WriteLine("\nFiles with error:");
                        foreach (string file in fileWithError)
                        {
                            Console.WriteLine(file);
                        }
                    }
                }

                #endregion Execute with simulator

            }
            catch (Exception e)
            {
                Utils.PrintError("unknow_error", false, e.ToString());
            }
        }

        #region Compilation Pipeline
        public static Dictionary<string, StringBuilder> LaunchCompilation(string path, StringBuilder sb, CodeGenerator codeGenerator, bool isDirectory = false, bool isJoined = false)
        {
            Dictionary<string, StringBuilder> assemblyCodes = new();

            if (isJoined)
            {
                assemblyCodes.Add(Path.GetFileName(path), sb);
            }

            return LaunchCompilation(path, assemblyCodes, codeGenerator, isDirectory, isJoined);
        }

        public static Dictionary<string, StringBuilder> LaunchCompilation(string path, Dictionary<string, StringBuilder> sbs, CodeGenerator codeGenerator, bool isDirectory = false, bool isJoined = false)
        {
            if (isDirectory)
            {
                string[] filepaths = Directory.GetFiles(path, "*.c", SearchOption.AllDirectories);

                foreach (string filepath in filepaths)
                {
                    LaunchCompilation(filepath, sbs, codeGenerator, false, isJoined);
                }
            }
            else
            {
                string fileName = Path.GetFileNameWithoutExtension(path);

                Console.WriteLine("----------------------------------------------------------------------------------------------------");
                Console.WriteLine($"\nStart compiling {fileName}");

                codeGenerator = new();
                codeGenerator.AddFileToLexical(Utils.LoadFileFromPath(path));
                if (Utils.runtimeMode)
                {
                    Utils.Libraries.Add(new(fileName));
                    Utils.currentLibrary = fileName;
                }

                if (isJoined)
                {
                    sbs.First().Value.AppendLine(codeGenerator.GenerateCode(fileName));
                }
                else
                    sbs.Add(fileName, new StringBuilder(codeGenerator.GenerateCode(fileName)));

                if (Utils.runtimeMode)
                {
                    Utils.Libraries.First(l => l.Name == fileName).AssemblyCode = sbs[fileName];
                    Utils.currentLibrary = string.Empty;
                }

                Console.WriteLine($"End compiling {fileName}\n");

                Console.WriteLine("----------------------------------------------------------------------------------------------------");
            }

            return sbs;
        }

        public static void LaunchMake()
        {
            string simulatorPath = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "msm");

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

            if (Utils.debugMode)
                Console.WriteLine($" Execute make final command : {makeProcess.StartInfo.FileName} {makeProcess.StartInfo.Arguments}");

            makeProcess.Start();
            makeProcess.WaitForExit();

            if (makeProcess.ExitCode != 0)
            {
                Utils.PrintError("make_failed", false, makeProcess.StandardError.ReadToEnd());
                return;
            }
        }

        public static int LaunchSimulator(KeyValuePair<string, StringBuilder> assemblyCode)
        {

            string tempFile = Path.GetTempFileName();
            string simulatorPath = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "msm");
            string logFile = Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log", $"log_{assemblyCode.Key}.txt");

            // Write the assembly code in the temporary file
            File.WriteAllText(tempFile, assemblyCode.Value.ToString());

            // Create the directory log
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "simulator", "log"));

            // Create a new process and redirect the output to the log file
            Process simulatorProcess = new()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(simulatorPath, "msm.exe"),
                    Arguments = $"-d -m {tempFile}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    CreateNoWindow = true
                }
            };

            if (Utils.debugMode)
                Console.WriteLine($" Execute simulator final command : {simulatorProcess.StartInfo.FileName} {simulatorProcess.StartInfo.Arguments}");

            // Start the process
            simulatorProcess.Start();
            simulatorProcess.WaitForExit();

            string outputStd = simulatorProcess.StandardOutput.ReadToEnd();
            string errorStd = simulatorProcess.StandardError.ReadToEnd();
            int exitCode = simulatorProcess.ExitCode;

            // Print error if the process failed
            if (exitCode != 0)
            {
                Utils.PrintError("simulator_failed", false, errorStd);
            }

            File.WriteAllText(logFile, FormatLog(assemblyCode.Value.ToString(), outputStd, errorStd));
            File.Delete(tempFile);

            // Print the log file path
            Console.WriteLine($"\nLog file: {logFile}");

            return exitCode;
        }

        private static string FormatLog(string entry, string output, string error)
        {
            StringBuilder sb = new();

            sb.AppendLine("------------------------------------------entry----------------------------------------------------------");
            sb.AppendLine(entry);
            sb.AppendLine("------------------------------------------output---------------------------------------------------------");
            sb.AppendLine(output);
            sb.AppendLine("------------------------------------------error----------------------------------------------------------");
            sb.AppendLine(error);

            return sb.ToString();
        }

        #endregion Compilation Pipeline
    }
}