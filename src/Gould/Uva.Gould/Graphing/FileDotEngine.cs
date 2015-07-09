using System.Diagnostics;
using System.IO;
using Uva.Gould.Properties;

namespace Uva.Gould.Graphing
{
    /// <summary>
    /// Writes dot to file and invokes external process which creates the graph png.
    /// </summary>
    public class FileDotEngine
    {
        public void Run(string dot, string outputFileName)
        {
            File.WriteAllText(outputFileName, dot);

            if (!File.Exists(Settings.Default.DotPath))
                throw new FileNotFoundException("Dot file executable not found at {0}", Settings.Default.DotPath);
            
            // Pass through to command line.
            var startInfo = new ProcessStartInfo(Settings.Default.DotPath,
                string.Format(@"{0} -Tpng -O", outputFileName))
            {
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            Process.Start(startInfo);
        }
    }
}
