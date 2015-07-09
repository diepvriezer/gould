using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;

namespace Uva.Gould.Graphing
{
    /// <summary>
    /// Crippled graph, only suitable for plotting to dot. Built out of frustration with QuickGraph.
    /// </summary>
    public class Graph<TVertex, TEdge>
        where TEdge : IEdge<TVertex>
    {
        public Graph(IEnumerable<TVertex> vertices, IEnumerable<TEdge> edges, bool isDirected = true)
        {
            IsDirected = isDirected;

            foreach (var v in vertices)
                AddVertex(v);
            foreach (var e in edges)
                AddEdge(e);

            GraphFormat = GraphStyles.DefaultGraphFormat();
            CommonEdgeFormat = GraphStyles.DefaultEdgeFormat();
            CommonVertexFormat = GraphStyles.DefaultVertexFormat();

            Writer = new FileDotEngine();
        }

        public delegate void VertexFormatter(TVertex vertex, ref dynamic format);
        public delegate void EdgeFormatter(TEdge edge, ref dynamic format);

        public event VertexFormatter FormatVertex;
        public event EdgeFormatter FormatEdge;

        public dynamic GraphFormat { get; set; }
        public dynamic CommonEdgeFormat { get; set; }
        public dynamic CommonVertexFormat { get; set; }
        public bool IsDirected { get; private set; }
        public string Name { get; set; }
        public FileDotEngine Writer { get; set; }

        private readonly static IFormatProvider usCulture = CultureInfo.GetCultureInfo("en-US");
        
        private Dictionary<TVertex, int> vertexMap = new Dictionary<TVertex, int>();
        private HashSet<TEdge> edges = new HashSet<TEdge>();
        private int nextId = 0;
        

        // Adding vertices/edges.
        private void AddVertex(TVertex vertex)
        {
            if (vertexMap.ContainsKey(vertex))
                throw new InvalidOperationException("Vertex already in graph!");

            vertexMap.Add(vertex, ++nextId);
        }
        private void AddEdge(TEdge edge)
        {
            if (edges.Contains(edge))
                throw new InvalidOperationException("Edge already in graph!");

            edges.Add(edge);
        }


        // DOT generation.
        public string GenerateDot()
        {
            using (var sw = new StringWriter(usCulture))
            {
                sw.WriteLine("{0}graph {1} {{", IsDirected ? "di" : null, Name);
                
                // Common graph settings.
                foreach (var line in EnumerateExpando(GraphFormat))
                    sw.WriteLine("{0};", line);

                sw.WriteLine("node {0};", JoinExpando(CommonVertexFormat));
                sw.WriteLine("edge {0};", JoinExpando(CommonEdgeFormat));

                // Iterate each vertex and print.
                foreach (var v in vertexMap.Keys)
                {
                    int id = vertexMap[v];

                    dynamic format = new ExpandoObject();
                    format.Label = id.ToString();

                    if (FormatVertex != null)
                        FormatVertex.Invoke(v, ref format);

                    sw.WriteLine(" {0} {1};", id, JoinExpando(format));
                }

                // Iterate each edge and print.
                foreach (var e in edges)
                {
                    int sourceId = vertexMap[e.Source];
                    int targetId = vertexMap[e.Target];

                    dynamic format = new ExpandoObject();

                    if (FormatEdge != null)
                        FormatEdge.Invoke(e, ref format);

                    sw.WriteLine(" {0} -> {1} {2};", sourceId, targetId, JoinExpando(format));
                }

                sw.WriteLine("}");

                return sw.ToString();
            }
        }
        public string GenerateDot(string path)
        {
            var dot = GenerateDot();
            Writer.Run(dot, path);
            return dot;
        }

        
        // Expando helpers.
        private static string JoinExpando(dynamic settings)
        {
            string result = string.Join(", ", EnumerateExpando(settings));
            if (string.IsNullOrEmpty(result))
                return null;
            else
                return "[" + result + "]";
        }
        private static IEnumerable<string> EnumerateExpando(dynamic settings)
        {
            foreach (var prop in (IDictionary<string, object>) settings)
                yield return string.Format(usCulture, "{0}={1}", prop.Key.ToLower(), FormatSetting(prop.Value));
        }
        private static object FormatSetting(object setting)
        {
            if (setting is Color)
            {
                var color = (Color) setting;
                return string.Format(usCulture, "\"#{0:X2}{1:X2}{2:X2}\"", color.R, color.G, color.B);
            }
            if (setting is bool)
            {
                return setting.ToString().ToLower();
            }
            if (setting is string)
            {
                return "\"" + setting + "\"";
            }

            return setting;
        }
    }
}
