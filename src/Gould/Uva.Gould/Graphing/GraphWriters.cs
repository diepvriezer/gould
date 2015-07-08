using System;
using System.Drawing;
using System.Globalization;
using System.Threading;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using Uva.Gould.Properties;

namespace Uva.Gould.Graphing
{
    /// <summary>
    /// Outputs dot files and uses GraphViz to create an output image.
    /// </summary>
    public class GraphWriter
    {
        private static GraphvizFont font = new GraphvizFont(Settings.Default.GraphFont, Settings.Default.GraphFontSize);
        private static GraphvizFont fontSmall = new GraphvizFont(Settings.Default.GraphFontSmall, Settings.Default.GraphFontSmallSize);

        // Generic case.
        public static void WriteToFile<TVertex, TEdge>(string path, IEdgeListGraph<TVertex, TEdge> graph)
            where TEdge : IEdge<TVertex>
        {
            var gv = new GraphvizAlgoDecorator<TVertex, TEdge>(graph, path);

            SetLocale();
            gv.Generate(new FileDotEngine(), path);
            ResetLocale();
        }

        // More specific styling.
        public static void WriteToFile(string path, IEdgeListGraph<GraphGenerator.AstVertex, GraphGenerator.AstEdge> graph)
        {
            var gv = new GraphvizAlgoDecorator<GraphGenerator.AstVertex, GraphGenerator.AstEdge>(graph, path);

            gv.FormatVertex += (sender, args) =>
            {
                var v = args.Vertex;
                var vf = args.VertexFormatter;

                vf.Label = v.Name;
//                
//                if (v.IsAbstract)
//                {
//                    vf.Style = GraphvizVertexStyle.Dashed;
//                }
//                if (v.IsRoot)
//                {
//                    
//                }
            };

            gv.FormatEdge += (sender, args) =>
            {
                var e = args.Edge;
                var ef = args.EdgeFormatter;

                if (e.IsInheritance)
                {
                    ef.Label.Value = null;
                }
                else
                {
                    ef.Label.Value = e.Name;
                }
            };

            SetLocale();
            gv.Generate(new FileDotEngine(), path);
            ResetLocale();
        }

        protected class GraphvizAlgoDecorator<TVertex, TEdge> : GraphvizAlgorithm<TVertex, TEdge>
            where TEdge : IEdge<TVertex>
        {
            public GraphvizAlgoDecorator(IEdgeListGraph<TVertex, TEdge> graph, string path)
                : base(graph, path, GraphvizImageType.Png)
            {
                CommonEdgeFormat.Font = fontSmall;
                CommonEdgeFormat.FontGraphvizColor = GraphvizColors.From(Color.Black);
                CommonEdgeFormat.StrokeGraphvizColor = GraphvizColor.Black;
                CommonEdgeFormat.Style = GraphvizEdgeStyle.Unspecified;

                CommonVertexFormat.Font = font;
                CommonVertexFormat.FontColor = GraphvizColor.Black;
                CommonVertexFormat.Shape = GraphvizVertexShape.Ellipse;
                CommonVertexFormat.Style = GraphvizVertexStyle.Filled;
                CommonVertexFormat.FillColor = GraphvizColors.From(Color.Gray);
                CommonVertexFormat.StrokeColor = GraphvizColors.From(Color.Gray);

                GraphFormat.RankSeparation = 0.8;
            }
        }

        private static void SetLocale()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        }

        private static void ResetLocale()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InstalledUICulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InstalledUICulture;
        }
    }
}
