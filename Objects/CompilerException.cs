using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class CompilerException
    {
        public string Code { get; set; }
        public string Message { get; set; }

        public CompilerException(string code, string errorMessage)
        {
            Code = code;
            Message = errorMessage;
        }

        public override string ToString()
        {
            return string.Format("[{0}] : {1}", Code, Message);
        }
    }
}
