using Uva.Gould;

namespace Uva.CivicCompiler.Ast
{
    public abstract class Expression : Node { }

    public class BinaryOperation : Expression
    {
        [Child] public Expression Left { get; set; }
        [Child] public Expression Right { get; set; }

        public BinOp Operator { get; set; }
    }
    public class Variable : Expression
    {
        public string Id { get; set; }
    }
    public class IntConst : Expression
    {
        public int Value { get; set; }
    }
    public class BoolConst : Expression
    {
        public bool Value { get; set; }
    }
    public class FloatConst : Expression
    {
        public float Value { get; set; }
    }
}
