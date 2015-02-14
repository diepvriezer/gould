using Uva.Gould;

namespace Uva.CivicCompiler.Ast
{
    public abstract class Statement : Node { }

    public class Assignment : Statement
    {
        [Child] public Expression Expression { get; set; }

        public string Id { get; set; }
    }
}
