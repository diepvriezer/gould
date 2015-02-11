using System;

namespace Uva.Gould.Graphs
{
    /// <summary>
    /// Edge in a graph, can be named and may have a specific style.
    /// </summary>
    public class GraphEdge
    {
        public GraphEdge(GraphNode head, GraphNode tail, string name = null, int style = 0)
        {
            if (head == null) throw new ArgumentNullException("head");
            if (tail == null) throw new ArgumentNullException("tail");

            Head = head;
            Tail = tail;
            Name = name;
            Style = style;
        }

        public string Name { get; set; }
        public int Style { get; set; }
        public GraphNode Head { get; protected set; }
        public GraphNode Tail { get; protected set; }
    }
}
