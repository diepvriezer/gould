namespace Uva.Gould.Tests.Fixtures
{
    public class IdNode : Node
    {
        public IdNode(int id)
        {
            Id = id;
        }

        public int Id { get; protected set; }

        public static int StartId = 0;

        public override string ToString()
        {
            return Id + " (" + GetType().Name + ")";
        }
    }

    // Non-generic, sometimes easier.
    public class BiNode : BiNode<BiNode, BiNode>
    {
        public BiNode(int id) : base(id) { }
    }
    // Binary node, i.e. max 2 children.
    public class BiNode<TLeft, TRight> : IdNode
        where TLeft : IdNode
        where TRight : IdNode
    {
        public BiNode(int id) : base(id) { }

        [Child] public TLeft Left { get; set; }
        [Child] public TRight Right { get; set; }
    }
    // Simple inheritance tests, so that A2 fits in an A0 slot, but not in B0, etc.
    public class BiNodeA0 : BiNode
    {
        public BiNodeA0(int id) : base(id) { }
    }
    public class BiNodeA1 : BiNodeA0
    {
        public BiNodeA1(int id) : base(id) { }
    }
    public class BiNodeA2 : BiNodeA1
    {
        public BiNodeA2(int id) : base(id) { }
    }
    public class BiNodeB0 : BiNode
    {
        public BiNodeB0(int id) : base(id) { }
    }


    /// <summary>
    ///       0
    ///      / \
    ///     1   4
    ///    / \
    ///   2   3
    /// </summary>
    public class BiNodeTree1 : BiNode
    {
        public BiNodeTree1() : base(0)
        {
            Left = new BiNode(1) { Left = new BiNode(2), Right = new BiNode(3) };
            Right = new BiNode(4);
        }
    }

    /// <summary>
    /// ID's:
    ///           0
    ///         /   \
    ///        1     4
    ///       / \   / \
    ///      2   3 5   6
    /// 
    /// Restricted by (# = any):
    ///         /    \
    ///        #     B0
    ///       / \    / \
    ///     A2   A1 #   #
    /// 
    /// Loaded with:
    ///         /    \
    ///        #     B0
    ///       / \    / \
    ///     A2   A2 A2  A2
    /// </summary>
    public class BiNodeTree2 : BiNode<BiNode<BiNodeA2, BiNodeA1>, BiNodeB0>
    {
        public BiNodeTree2() : base(0)
        {
            Left = new BiNode<BiNodeA2, BiNodeA1>(1) {Left = new BiNodeA2(2), Right = new BiNodeA2(3)};
            Right = new BiNodeB0(4) { Left = new BiNodeA2(5), Right = new BiNodeA2(6)};
        }
    }
}
