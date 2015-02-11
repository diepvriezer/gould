using System;

namespace Uva.Gould.Graphs
{
    /// <summary>
    /// Node in a graph, can be named and may have a specific type.
    /// </summary>
    public class GraphNode
    {
        public GraphNode(string id, string name = null, int style = 0)
        {
            if (id == null) throw new ArgumentNullException("id");
            Id = id;
            Name = name;
            Style = style;
        }

        public string Id { get; private set; }
        public string Name { get; set; }
        public int Style { get; set; }
    }
}
