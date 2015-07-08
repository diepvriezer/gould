using System.Collections.Generic;
using System.Drawing;
using QuickGraph.Graphviz.Dot;

namespace Uva.Gould.Graphing
{
    public static class GraphvizColors
    {
        private static Dictionary<Color, GraphvizColor> colors = new Dictionary<Color, GraphvizColor>(); 

        public static GraphvizColor From(KnownColor color)
        {
            return From(Color.FromKnownColor(color));
        }

        public static GraphvizColor From(Color color)
        {
            if (!colors.ContainsKey(color))
                colors.Add(color, new GraphvizColor(color.A, color.R, color.G, color.B));

            return colors[color];
        }
    }

    public struct GraphvizColorFixed : GraphvizColor
    {
        
    }
}
