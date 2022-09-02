namespace CompilerC__
{
    internal class Program
    {

        static void Main(string[] args)
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
                Utils.PrintError("invalid_file_extension", filePath);
                return;
            }

            if (args.Length > 1)
            {
                if (args[1] == "--debug")
                {
                    Utils.debugMode = true;
                }
            }

            #endregion Parse arguments

            #region Load File

            Console.WriteLine("\nStart file loading");

            if (!File.Exists(filePath))
            {
                Utils.PrintError("file_not_exist", filePath);
                return;
            }

            List<string>? fileLines = null;
            try
            {
                fileLines = File.ReadLines(filePath).ToList();
            }
            catch (System.Exception e)
            {
                Utils.PrintError("file_read_error", e.Message);
            }

            if (fileLines == null || fileLines.Count == 0)
            {
                Utils.PrintError("file_empty");
            }

            Console.WriteLine("File loading finished");

            #endregion Load File

            #region Compile

            Console.WriteLine("\nStart compilation");

            // Call related methods
            /*
             static currentToken and prevToken;
            next()
             */
            #region Lexical Scanner

            Console.WriteLine("\nStart lexical scanner");

            LexicalScanner lexicalScanner = new LexicalScanner(fileLines);

            string[] tokenType = new string[] {};
            while (true)
            {
                tokenType = lexicalScanner?.Next()?.Select(t => t.Type).ToArray();

                if (tokenType != null && tokenType.Length > 0 && Utils.debugMode)
                {
                    Console.WriteLine($"token found : {tokenType.Aggregate((a, b) => $"{a} {b}")}\n");
                }


                if (tokenType.Any(t => t == Utils.tokenTypes[0].Code))
                    break;

                System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine("Lexical scanner finished");

            #endregion Lexical Scanner

            Console.WriteLine("\nCompilation finished");

            #endregion Compile
        }
    }
}