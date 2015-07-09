using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using Uva.Gould.Graphing.Elements;

namespace Uva.Gould.Graphing
{
    public class GraphStyles
    {
        protected static readonly Color[,] colors =
        {
            {Color.DarkKhaki, Color.PapayaWhip, Color.Black},
            {Color.DarkGreen, Color.DarkSeaGreen, Color.White},
            {Color.RoyalBlue, Color.LightBlue, Color.DarkSlateGray},
            {Color.DarkCyan, Color.LightCyan, Color.DarkSlateGray}
        };

        public static void SetupAstGraphStyles(Graph<AstVertex, AstEdge> graph)
        {
            graph.Name = "AST";
            graph.FormatVertex += (AstVertex vertex, ref dynamic format) =>
            {
                format.Label = vertex.Name;
                if (vertex.IsRoot)
                {
                    format.FontSize = 21.0;
                    SetVertexColors(format, 1);
                }
                else if (vertex.IsAbstract || vertex.DerivedTypes.Any())
                {
                    format.Margin = "0.60,0.22";
                    format.Shape = "rectangle";
                    format.Style = "dashed, filled, bold";

                    SetVertexColors(format, vertex.IsAbstract ? 2 : 3);
                }
            };

            graph.FormatEdge += (AstEdge edge, ref dynamic format) =>
            {
                if (edge.IsInheritance)
                {
                    SetInheritanceArrow(format);
                    SetEdgeColors(format, edge.Source.IsAbstract ? 2 : 3);
                }
                else
                {
                    format.Label = edge.Name;
                    format.ArrowSize = 0.5;
                    SetDotLineArrow(format);
                }
            };
        }


        // Default styling.
        public static dynamic DefaultGraphFormat()
        {
            dynamic format = new ExpandoObject();
            format.RankSep = 0.95;
            format.NodeSep = 0.95;
            format.Center = true;
            format.Ratio = "auto";
            format.Margin = 0;
            format.SearchSize = 40;

            return format;
        }

        public static dynamic DefaultEdgeFormat()
        {
            dynamic format = new ExpandoObject();
            format.FontName = "tahoma";
            format.FontSize = 13.0;
            format.Style = "solid";

            format.FontColor = Color.DarkSlateGray;
            SetEdgeColors(format, 0);

            return format;
        }

        public static dynamic DefaultVertexFormat()
        {
            dynamic format = new ExpandoObject();
            format.FontName = "tahoma bold";
            format.FontSize = 18;

            format.Margin = "0.30,0.16";
            format.Shape = "ellipse";
            format.Style = "solid, filled, bold";

            SetVertexColors(format, 0);
            
            return format;
        }


        // Clean up type names.
        public static string GetCleanTypeName(Type t)
        {
            string name = t.Name;
            if (t.IsGenericType)
            {
                name =
                    t.Name.Substring(0, t.Name.IndexOf('`'))
                    + "<" + string.Join(", ", t.GetGenericArguments().Select(GetCleanTypeName))
                    + ">";
            }

            return name;
        }

        
        // Color helpers.
        private static void SetVertexColors(dynamic format, int ix)
        {
            format.Color = colors[ix, 0];
            format.StrokeColor = colors[ix, 1];
            format.FillColor = colors[ix, 1];
            format.FontColor = colors[ix, 2];
        }

        private static void SetEdgeColors(dynamic format, int ix)
        {
            format.Color = colors[ix, 0];
        }

        private static void SetDotLineArrow(dynamic format)
        {
            format.Dir = "both";
            format.ArrowTail = "dot";
            format.ArrowHead = "normal";
        }

        private static void SetInheritanceArrow(dynamic format)
        {
            format.Dir = "both";
            format.ArrowTail = "vee";
            format.ArrowHead = "dot";
            format.Style = "dashed";
        }
    }
}
