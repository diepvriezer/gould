using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using Uva.Gould.Phases;

namespace Uva.Gould.Support
{
    /// <summary>
    /// Provides helpers for command line output and pipeline halting.
    /// </summary>
    public static class CTI
    {
        #region Print functions

        public static void Debug(string message, params object[] args)
        {
            if (EnableDebug)
                PrintNoIndent(message, args);
        }
        public static void Debug(Exception ex)
        {
            if (EnableDebug)
                PrintNoIndent(ex.ToString());
        }

        public static void Info(string message, params object[] args)
        {
            PrintNoIndent("Info: " + message, args);
        }
        public static void InfoWithSource(string message, SourceInfo sourceInfo, params object[] args)
        {
            if (args != null && args.Any())
                Info(FormatOriginMessage(sourceInfo, string.Format(message, args)));
            else
                Info(FormatOriginMessage(sourceInfo, message));
        }

        public static void Warn(string message, params object[] args)
        {
            PhaseRunner.PhaseWarnings++;
            PhaseRunner.TotalWarnings++;
            PrintNoIndent("Warning: " + message, args);
        }
        public static void WarnWithSource(string message, SourceInfo sourceInfo, params object[] args)
        {
            if (args != null && args.Any())
                Warn(FormatOriginMessage(sourceInfo, string.Format(message, args)));
            else
                Warn(FormatOriginMessage(sourceInfo, message));
        }

        public static void Error(string message, params object[] args)
        {
            PhaseRunner.PhaseErrors++;
            PhaseRunner.TotalErrors++;
            PrintNoIndent("Error: " + message, args);
        }
        public static void ErrorWithSource(string message, SourceInfo sourceInfo, params object[] args)
        {
            if (args != null && args.Any())
                Error(FormatOriginMessage(sourceInfo, string.Format(message, args)));
            else
                Error(FormatOriginMessage(sourceInfo, message));
        }
        
        public static void Critical(string message, params object[] args)
        {
            PhaseRunner.AbortCompilation = true;
            Error(message, args);
        }
        public static void CriticalWithSource(string message, SourceInfo sourceInfo, params object[] args)
        {
            PhaseRunner.AbortCompilation = true;
            ErrorWithSource(message, sourceInfo, args);
        }

        #endregion
        
        // Indented writer wrapper.
        private static IndentedTextWriter _indentedTextWriter;
        private static TextWriter _originalOut;

        public static bool EnableDebug = false;

        public static int Indent
        {
            get { return _indentedTextWriter.Indent; }
            set { _indentedTextWriter.Indent = value; }
        }

        /// <summary>
        /// Attaches an indent writer to the std out.
        /// </summary>
        public static void AttachIndentWriter(string tabString = "  ")
        {
            if (_indentedTextWriter == null)
            {
                _originalOut = Console.Out;
                _indentedTextWriter = new IndentedTextWriter(Console.Out, tabString);
                Console.SetOut(_indentedTextWriter);
            }
        }

        /// <summary>
        /// Detaches the indent writer.
        /// </summary>
        public static void DetachIndentWriter()
        {
            if (_indentedTextWriter != null)
            {
                Console.SetOut(_originalOut);
                _originalOut = null;
                _indentedTextWriter.Dispose();
                _indentedTextWriter = null;
            }
        }


        private static void PrintNoIndent(string message, params object[] args)
        {
            int oldIndent = Indent;
            Indent = 0;
            Print(message, args);
            Indent = oldIndent;
        }
        private static void Print(string message, params object[] args)
        {
            if (args == null || args.Length == 0)
                Console.WriteLine(message);
            else
                Console.WriteLine(message, args);
        }
        private static string FormatOriginMessage(SourceInfo origin, string message)
        {
            if (origin == null)
                return message;

            var sb = new StringBuilder();
            sb.AppendLine(string.Format("{2} at line {0}, column {1}", origin.LineNumber, origin.Column, message));
            sb.AppendLine(origin.TargetLine);
            sb.Append(GetIndent(origin.TargetLine, origin.Column));
            sb.Append("^");

            return sb.ToString();
        }
        private static string GetIndent(string line, int col)
        {
            string sub = line.Substring(0, col);

            string indent = "";
            for (int i = 0; i < sub.Length; i++)
            {
                indent += sub[i] == '\t' ? "\t" : " ";
            }

            return indent;
        }
    }
}
