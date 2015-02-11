using System;
using System.Diagnostics;
using System.IO;

namespace Uva.Gould.Graphs
{
    public static class DotPngFileWriter
    {
        /// <summary>
        /// Path to dot executable.
        /// </summary>
        public const string DotPath = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
        
        /// <summary>
        /// Writes a .DOT and .PNG file to disk.
        /// </summary>
        /// <param name="dot">.dot formatted text</param>
        /// <param name="dotOutputPath">Path or filename</param>
        /// <param name="pngOutputPath">Optional, defaults to same as .dot but with .png</param>
        public static void Write(string dot, string dotOutputPath, string pngOutputPath = null)
        {
            if (dotOutputPath == null) throw new ArgumentNullException("dotTarget");
            if (pngOutputPath == null)
            {
                pngOutputPath = Path.Combine(Path.GetDirectoryName(dotOutputPath),
                    Path.GetFileNameWithoutExtension(dotOutputPath) + ".png");
            }

            if (!File.Exists(DotPath))
                throw new FileNotFoundException("Dot file executable not found at {0}", DotPath);

            using (var io = new StreamWriter(dotOutputPath, false))
                io.Write(dot);

            // Pass through to command line.
            var startInfo = new ProcessStartInfo(DotPath,
                string.Format(@"""{0}"" -o ""{1}"" -Tpng", dotOutputPath, pngOutputPath));
            startInfo.CreateNoWindow = true;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            Process.Start(startInfo);
        }
    }
}
