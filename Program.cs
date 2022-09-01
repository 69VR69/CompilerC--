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
                Utils.printError("invalid_file_extension",new object[] { filePath });
                return;
            }

            #endregion Parse arguments

            Console.WriteLine("Start compilation");

            // Call related methods
            /*
             static currentToken and prevToken;
            next()
             */

            Console.WriteLine("Compilation finished");
        }
    }
}