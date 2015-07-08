// Sample AST nodes to avoid abstract naming.
namespace Uva.Gould.Tests.Fixtures
{
    // A node with an auto incremented ID.
    public class IncNode
    {
        public IncNode()
        {
            Id = StartId++;
        }
        
        public int Id { get; protected set; }

        public static int StartId = 0;

        public override string ToString()
        {
            return Id + " (" + GetType().Name + ")";
        }
    }

    /*
     * Simple AST-like hierarchy:
     * 
     * IncNode
     *  |
     *  |-- Expression
     *  |----|
     *  |----|-- BinOp
     *  |----|-- Int
     *  |----|-- Bool
     *  |
     *  |-- Statement
     *  |----|
     *  |----|-- If
     *  |----|-- For<T> (abstract)
     *  |----|----|
     *  |----|----|-- ForFixed (bounds are int only)
     *  |----|----|-- For (bounds are expression)
     *  |----|-- Block
     *  |
     *  |-- FunctionBody
     * 
     * The block statement was introduced to chain multiple statements together,
     * this would normally be done with an SSL-like structure.
     * 
     * Function body can only hold a block structure rather than any statement,
     * this is done for specific test cases.
     */

    // Expression is a concrete here, usually it's abstract
    public class Expression : IncNode { }
    public class BinOp : Expression
    {
        [Child] public Expression Left { get; set; }
        [Child] public Expression Right { get; set; }
    }
    public class Int : Expression
    {
        public int Value { get; set; }
    }
    public class Bool : Expression
    {
        public bool Value { get; set; }
    }

    public class Statement : IncNode { }
    public class If : Statement
    {
        [Child] public Statement Then { get; set; }
        [Child] public Statement Else { get; set; }
    }
    public abstract class For<T> : Statement
        where T : Expression
    {
        [Child] public T From { get; set; }
        [Child] public T To { get; set; }
        [Child] public T Step { get; set; }
        [Child] public Statement Do { get; set; }
    }
    public class ForFixed : For<Int> { }
    public class For : For<Expression> { }

    public class Block : Statement
    {
        [Child] public Statement Statement1 { get; set; }
        [Child] public Statement Statement2 { get; set; }
        [Child] public Statement Statement3 { get; set; }
    }

    public class FunctionBody : IncNode
    {
        [Child] public Block Block { get; set; }
        [Child] public Expression Return { get; set; }
    }

    // Sample trees.
    public static class Trees
    {
        public static BinOp IntegerBinOp()
        {
            return new BinOp { Left = new Int(), Right = new Int() };
        }

        public static BinOp IntegerBooleanBinOp()
        {
            return new BinOp { Left = new Int(), Right = new Bool() };
        }

        public static BinOp NestedBinOps()
        {
            return new BinOp
            {
                Left = IntegerBooleanBinOp(),
                Right = IntegerBooleanBinOp()
            };
        }

        public static ForFixed ForFixed()
        {
            return new ForFixed
            {
                From = new Int(),
                To = new Int(),
                Step = new Int(),
                Do = new Statement()
            };
        }

        public static For For()
        {
            return new For
            {
                From = new Int(),
                To = new Int(),
                Step = new Int(),
                Do = new Statement()
            };
        }

        public static Block BlockWithStatements()
        {
            return new Block
            {
                Statement1 = new Statement(),
                Statement2 = new Statement(),
                Statement3 = new Statement()
            };
        }

        /* Block(0)
         *   ForFixed(1)
         *     int(2) int(3) int(4) stat(5)
         *   For(6)
         *     int(7) int(8) int(9) stat(10)
         *   Block(11)
         *     stat(12) stat(13) stat(14)
         */
        public static Block ForFixedForAndStatementBlock()
        {
            return new Block
            {
                Statement1 = ForFixed(),
                Statement2 = For(),
                Statement3 = BlockWithStatements()
            };
        }
    }
}
