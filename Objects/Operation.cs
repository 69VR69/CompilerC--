using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CompilerC__.Objects.Types;

namespace CompilerC__.Objects
{
    internal class Operation
    {
        public TokenType TokenType { get; set; }

        public int Priority { get; set; }

        public bool IsLeftAssociate { get; set; }

        public NodeType? NodeType { get; set; }

        public Operation(TokenType tokenType,int priority = 0, bool isleftAssociate = true, NodeType? nodeType = null)
        {
            TokenType = tokenType;
            Priority = priority;
            IsLeftAssociate = isleftAssociate;
            NodeType = nodeType;
        }
    }
}
