using Uva.Gould;

namespace Uva.CivicCompiler.Ast
{
    public class Program : Node
    {
        [Child] public LinkedNode<Statement> Statements { get; set; }
    }
}
