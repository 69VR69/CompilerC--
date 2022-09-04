using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompilerC__.Objects
{
    internal class ElementGroup
    {
        public GrammarGroup Category { get; set; }
        public int Order { get; set; }
        public Element[] Elements { get; set; }
        public NodeType[] NodeTypes { get; set; }
        public Node Node { get; set; }

        public ElementGroup(GrammarGroup category,int order, Element[] elements, NodeType[] nodeTypes)
        {
            Category = category;
            Order = order;
            Elements = elements;
            NodeTypes = nodeTypes;
        }

        public ElementGroup(GrammarGroup category,int order, Element[] elements, Node node)
        {
            Category = category;
            Order = order;
            Elements = elements;
            Node = node;
        }
    }
}
