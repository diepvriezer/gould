using System;
using System.Collections.Generic;
using System.Reflection;
using Uva.Gould.Support;

namespace Uva.Gould.Phases
{
    public class PhaseRunner
    {
        public PhaseRunner()
        {
            Phases = new List<IPhase>();
        }
        
        // Phase counters and vars.
        public static int PhaseIndex = 0;
        public static int PhaseErrors = 0;
        public static int PhaseWarnings = 0;
        public static int TotalWarnings = 0;
        public static int TotalErrors = 0;
        public static bool AbortCompilation = false;

        public List<IPhase> Phases { get; private set; }
        
        public void Run()
        {
            PhaseErrors = PhaseWarnings = 0;

            // Root AST node, to be filled in first phase.
            Node ast = null;

            // Cycle through phases, abort on error.
            bool aborted = false;
            for (int i = 0; i < Phases.Count; i++)
            {
                PhaseIndex = i;

                var phase = Phases[i];
                var phaseName = GetPhaseName(phase);

                CTI.Debug("Phase #" + i + " - " + phaseName);
                CTI.Indent++;

                // Reset error count.
                PhaseErrors = PhaseWarnings = 0;
                AbortCompilation = false;

                // Run phase, catch exceptions in release.
#if DEBUG
                ast = phase.Transform(ast);
#else
                try
                {
                    ast = phase.Transform(ast);
                }
                catch (Exception ex)
                {
                    CTI.Critical(ex.ToString());
                }
#endif

                CTI.Indent--;
                Console.WriteLine();

                // Report if compilation is aborted.
                if (AbortCompilation || TotalErrors > 0)
                {
                    aborted = true;
                    break;
                }
            }

            // Report compilation status.
            Console.WriteLine("==== Compilation {2} {0} errors, {1} warnings ====",
                TotalErrors, TotalWarnings, aborted ? "aborted." : "succeeded!");
        }
        

        private string GetPhaseName(object obj)
        {
            var t = obj.GetType();
            var attrib = t.GetCustomAttribute<PhaseAttribute>();
            return attrib != null
                ? attrib.Name
                : t.Name;
        }
    }
}
