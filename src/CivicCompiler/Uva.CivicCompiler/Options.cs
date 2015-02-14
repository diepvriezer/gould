using CommandLine;
using CommandLine.Text;

namespace Uva.CivicCompiler
{
    class Options
    {
        [Option('i', "input", Required = true, HelpText = "Path to Civic langauge input file.")]
        public string InputPath { get; set; }

        [Option('o', "output", HelpText = "Path to compiled output file.")]
        public string OutputPath { get; set; }

        [Option('v', "verbose", HelpText = "Outputs more information during compilation.")]
        public bool Verbose { get; set; }
        
        [ParserState]
        public IParserState ParserState { get; set; }
        
        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, text => HelpText.DefaultParsingErrorsHandler(this, text));
        }
    }
}
