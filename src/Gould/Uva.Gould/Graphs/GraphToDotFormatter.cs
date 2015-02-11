using System;
using System.Text;

namespace Uva.Gould.Graphs
{
    public enum GraphDirection { Undirected, Directed }
    public enum GraphOrientation { TopBottom, LeftRight }

    /// <summary>
    /// Converts simple graphs into .dot text, applies basic styles.
    /// </summary>
    public class GraphToDotFormatter
    {
        public static readonly string[] EdgeStyles = { "style=dashed,color=blue;", "style=bold,color=green;", "style=dashed,color=red" };
        public static readonly string[] NodeStyles = {"style=filled,color=lightgrey", "style=bold", "style=dashed,shape=circle,color=red,fontsize=8"};

        /// <summary>
        /// Default drawing settings. If the graph looks wrong, try setting reverse pointers to true.
        /// </summary>
        public string Format(Graph g, 
            GraphDirection direction = GraphDirection.Directed, 
            GraphOrientation orientation = GraphOrientation.TopBottom,
            bool reversePointers = false)
        {
            var pointers = direction == GraphDirection.Directed
                ? reversePointers
                    ? "arrowhead=none;arrowtail=open"
                    : "arrowhead=open;arrowtail=none"
                : "arrowhead=none;arrowtail=none";

            // Default node function.
            Func<GraphNode, string> nodeFunc = n =>
            {
                string extra = "label=\"" + (n.Name ?? n.Id) + "\";";
                if (n.Style != 0)
                    extra += PickStyle(NodeStyles, n.Style);

                return string.Format("\t{0} [{1}]", n.Id, extra);
            };
            
            // Default edge function.
            Func<GraphEdge, string> edgeFunc = e =>
            {
                string extra = "";
                if (e.Name != null)
                    extra += "label=\"" + e.Name + "\";";

                if (e.Style != 0)
                    extra += PickStyle(EdgeStyles, e.Style);

                return string.Format("\t{0} -> {1} [dir=both;{3};{2}]", e.Tail.Id, e.Head.Id, extra, pointers);
            };

            // Default initializer.
            Func<string> init = () =>
            {
                return "\tnode [shape=ellipse,fontname=\"Helvetica bold\",fontcolor=grey4,fontsize=13]" + Environment.NewLine +
                       "\tedge [style=solid,fontname=\"Helvetica\",fontcolor=grey4,fontsize=12]";

            };

            return Format(g, nodeFunc, edgeFunc, init);
        }

        /// <summary>
        /// Custom drawing functions.
        /// </summary>
        public string Format(Graph g, Func<GraphNode, string> nodeFunc, Func<GraphEdge, string> edgeFunc, Func<string> init = null)
        {
            if (g == null) throw new ArgumentNullException("g");
            if (nodeFunc == null) throw new ArgumentNullException("nodeFunc");
            if (edgeFunc == null) throw new ArgumentNullException("edgeFunc");

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("digraph {0} ", SafeName(g.Name)) + "{");
            if (init != null)
                sb.AppendLine(init.Invoke());

            g.Nodes.ForEach(n => sb.AppendLine(nodeFunc.Invoke(n)));
            g.Edges.ForEach(e => sb.AppendLine(edgeFunc.Invoke(e)));

            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string PickStyle(string[] from, int style)
        {
            return from[Math.Abs(--style)%from.Length];
        }
        private static string SafeName(string title)
        {
            return (title ?? "graph").ToAscii().ToLower().Replace(' ', '_');
        }
    }
}
