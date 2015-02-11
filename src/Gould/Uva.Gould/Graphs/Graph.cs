using System.Collections.Generic;

namespace Uva.Gould.Graphs
{
    /// <summary>
    /// Simple graph container.
    /// </summary>
    public class Graph
    {
        public Graph(string name)
        {
            Edges = new List<GraphEdge>();
            Nodes = new List<GraphNode>();
            Name = name;
        }

        public string Name { get; set; }
        public List<GraphEdge> Edges { get; protected set; }
        public List<GraphNode> Nodes { get; protected set; }
    }
}
