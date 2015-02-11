using System;
using System.IO;
using Uva.CivicCompiler.Ast;
using Uva.CivicCompiler.Phases;
using Uva.CivicCompiler.Phases.ScanParse;
using Uva.Gould.Graphs;
using Uva.Gould.Phases;
using Uva.Gould.Support;

namespace Uva.CivicCompiler
{
    class Compiler
    {
        private static void Main(string[] args)
        {
            // Initialize CTI indentation.
            CTI.AttachIndentWriter();

            // Each run we'll spit out the AST and inheritance tree images.
            CreateAstAndInheritanceGraphs();

            // Parse command line arguments.
            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options))
                return;

            // Set output path if missing.
            if (options.OutputPath == null)
            {
                var outputFile = Path.GetFileNameWithoutExtension(options.InputPath) + ".out";
                options.OutputPath = Path.Combine(Path.GetDirectoryName(options.InputPath), outputFile);
            }

            // Set global vars.
            CTI.EnableDebug = options.Verbose;
            
            // Create compiler and recurring phases.
            var compiler = new PhaseRunner();

            var print = new PrintTree();
            var graph = new GraphTree(options.OutputPath);

            compiler.Phases.AddRange(new IPhase[]
            {
                new ParseInputPhase(options.InputPath),
                print, graph
            });
            
            // Run phases.
            compiler.Run();

            Console.ReadKey();
        }

        private static void CreateAstAndInheritanceGraphs()
        {
            var graphs = new GraphBuilder();
            var formatter = new GraphToDotFormatter();

            DotPngFileWriter.Write(formatter.Format(graphs.CreateAstGraph<Program>()), "ast.dot");
            DotPngFileWriter.Write(formatter.Format(graphs.CreateInheritanceGraph(), reversePointers: true), "ast-inheritance.dot");
        }
    }
}
