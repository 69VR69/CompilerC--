using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class NodeType
    {
        public string Code { get; set; }

        public string[] AssemblyInstructions
        {
            get; set;
        }

        public NodeType(string code, params string[] assemblyInstructions)
        {
            Code = code;
            AssemblyInstructions = assemblyInstructions;
        }
    }
}
