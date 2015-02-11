using System;
using System.Linq;
using Antlr4.Runtime;

namespace Uva.Gould.Support
{
    /// <summary>
    /// Attached to the antlr parser, provides the rule invocation stack on syntax error.
    /// </summary>
    public class RuleStackErrorListener : BaseErrorListener
    {
        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line,
            int charPositionInLine, string msg, RecognitionException e)
        {
            var ruleStack = ((Parser)recognizer)
                .GetRuleInvocationStack()
                .Reverse()
                .ToList();

            CTI.Critical(msg + Environment.NewLine + "at line: " + line + ":" + charPositionInLine);
            CTI.Debug("Rule invocation stack:");
            ruleStack.ForEach(r => CTI.Debug("-> " + r));
        }
    }
}
