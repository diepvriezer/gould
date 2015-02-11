using System.Data;
using System.IO;
using Uva.Gould.Graphs;
using Uva.Gould.Support;

namespace Uva.Gould.Phases
{
    [Phase("Graph current tree")]
    public class GraphTree : IPhase
    {
        public GraphTree(string outputPath)
        {
            OutputPath = outputPath;
        }

        public string OutputPath { get; set; }

        private readonly GraphBuilder _builder = new GraphBuilder();
        private readonly GraphToDotFormatter _formatter = new GraphToDotFormatter();

        public Node Transform(Node root)
        {
            if (OutputPath == null) throw new NoNullAllowedException("Output path must be set");

            if (root == null)
            {
                CTI.Warn("Unable to print node graph, phase received null");
                return null;
            }

            var g = _builder.CreateObjectGraph(root);
            var dot = _formatter.Format(g);
            var outputDotPath = Path.Combine(
                Path.GetDirectoryName(OutputPath),
                Path.GetFileNameWithoutExtension(OutputPath) + "_" + PhaseRunner.PhaseIndex + ".dot");
            DotPngFileWriter.Write(dot, outputDotPath);

            CTI.Debug("Current AST graph written to DOT and PNG!");

            return root;
        }
    }
}
