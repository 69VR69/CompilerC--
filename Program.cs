namespace CompilerC__
{
    internal class Program
    {

        static void Main(string[] args)
        {
            #region Parse arguments

            if (args.Length == 0)
            {
                Utils.printError("invalid_argument");
                return;
            }

            string filePath = args[0];
            if (!filePath.EndsWith(".c"))
            {
                Utils.printError("invalid_file_extension", filePath);
                return;
            }

            #endregion Parse arguments

            #region Load File

            Console.WriteLine("Start file loading");

            if (!File.Exists(filePath))
            {
                Utils.printError("file_not_exist", filePath);
                return;
            }

            List<string>? fileLines = null;
            try
            {
                fileLines = File.ReadLines(filePath).ToList();
            }
            catch (System.Exception e)
            {
                Utils.printError("file_read_error", e.Message);
            }

            if (fileLines == null || fileLines.Count == 0)
            {
                Utils.printError("file_empty");
            }

            Console.WriteLine("File loading finished");

            #endregion Load File

            #region Compile

            Console.WriteLine("Start compilation");

            // Call related methods
            /*
             static currentToken and prevToken;
            next()
             */

            LexicalScanner lexicalScanner = new LexicalScanner(fileLines);

            string[] tokenType = new string[] {};
            while (true)
            {
                tokenType = lexicalScanner?.Next()?.Select(t => t.Type).ToArray();

                if (tokenType != null && tokenType.Count() > 0)
                {
                    Console.WriteLine($"token found : {tokenType.Aggregate((a, b) => $"{a} {b}")}\n");
                }


                if (tokenType.Any(t => t == Utils.tokenTypes[0].Code))
                    break;

                System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine("Compilation finished");

            #endregion Compile
        }
    }
}