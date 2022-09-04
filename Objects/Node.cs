using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class Node
    {

        #region Properties

        public string Type { get; set; }
        public int? Value { get; set; }
        public int Line { get; set; }
        public int Column { get; set; }
        public List<Node> Childs { get; set; }

        #endregion Properties

        #region Constructors

        public Node()
        {
            Type = string.Empty;
            Value = null;
            Line = 0;
            Column = 0;
            Childs = new List<Node>();
        }
        
        public Node(params Node[] childs)
        {
            Type = string.Empty;
            Value = null;
            Line = 0;
            Column = 0;
            Childs = childs.ToList();
        }
        
        public Node(string type,params Node[] childs)
        {
            Type = type;
            Value = null;
            Line = 0;
            Column = 0;
            Childs = childs.ToList();
        }

        public Node(string type, int line, int column)
        {
            Type = type;
            Value = null;
            Line = line;
            Column = column;
            Childs = new List<Node>();
        }

        public Node(string type, int value, int line, int column)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
            Childs = new List<Node>();
        }

        public Node(string type, int line, int column, params Node[] childs)
        {
            Type = type;
            Value = null;
            Line = line;
            Column = column;
            Childs = childs.ToList();
        }

        public Node(string type, int value, int line, int column, params Node[] childs)
        {
            Type = type;
            Value = value;
            Line = line;
            Column = column;
            Childs = childs.ToList();
        }
       
        #endregion Constructors
    }
}
