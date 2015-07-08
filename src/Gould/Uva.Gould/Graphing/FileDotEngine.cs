using System.Diagnostics;
using System.IO;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using Uva.Gould.Properties;

namespace Uva.Gould.Graphing
{
    public class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            dot = ReplaceColorTags(dot);

            File.WriteAllText(outputFileName, dot);

            if (!File.Exists(Settings.Default.DotPath))
                throw new FileNotFoundException("Dot file executable not found at {0}", Settings.Default.DotPath);

            // Image type switch.
            string fmt = "png";
            if (imageType == GraphvizImageType.Jpeg)
                fmt = "jpg";

            // Pass through to command line.
            var startInfo = new ProcessStartInfo(Settings.Default.DotPath,
                string.Format(@"{0} -T{1} -O", outputFileName, fmt))
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);
            return dot;
        }

        private static string ReplaceColorTags(string dot)
        {
            return dot
                .Replace("fontGraphvizColor", "fontcolor")
                .Replace("bgGraphvizColor", "fillcolor")
                .Replace("GraphvizColor", "color");
        }
    }
}
