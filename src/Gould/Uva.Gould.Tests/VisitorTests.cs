using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class VisitorTests
    {
        public class SwapsIntsWithBools : Visitor
        {
            public int Replacements { get; set; }

            public Expression Visit(Int node)
            {
                Replacements++;
                return new Bool();
            }
        }
        public class SwapsIntsWithBoolsExtended : SwapsIntsWithBools
        {
            public Int Visit(Expression node)
            {
                Replacements++;
                return new Int();
            }
        }

        public class RemoveAllBlocks : Visitor
        {
            public Block Visit(Block node)
            {
                return null;
            }
        }

        public class RemoveAllStatementsExceptBlocksAndFor : Visitor
        {
            public Block Visit(Block node)
            {
                VisitChildren(node);
                return node;
            }

            public For Visit(For node)
            {
                VisitChildren(node);
                return node;
            }

            public Statement Visit(Statement node)
            {
                return null;
            }
        }

        [TestMethod]
        public void VisitsChildNodes()
        {
            // Binop with two ints both set to 0.
            IncNode.StartId = 0;
            var tree = Trees.IntegerBinOp();

            var visitor = new SwapsIntsWithBools();
            Assert.IsTrue(tree.Left.Id == 1 && tree.Right.Id == 2);
            visitor.Visit(tree);

            Assert.IsNotNull(tree.Left);
            Assert.IsNotNull(tree.Right);
            Assert.IsInstanceOfType(tree.Left, typeof(Bool));
            Assert.IsInstanceOfType(tree.Right, typeof(Bool));
            Assert.IsTrue(tree.Left.Id == 3 && tree.Right.Id == 4);
            Assert.IsTrue(visitor.Replacements == 2);
        }

        [TestMethod]
        public void DoesNotReplaceIfNotApplicable()
        {
            // Fixed for with ints loaded and empty statement.
            var tree = Trees.ForFixed();

            var visitor = new SwapsIntsWithBools();
            visitor.Visit(tree);

            Assert.IsTrue(visitor.Replacements == 0);

            // Complex tree example, fixed tree does not replace ints.
            var complexTree = Trees.ForFixedForAndStatementBlock();
            visitor.Visit(complexTree);

            Assert.IsTrue(visitor.Replacements == 3);
        }

        [TestMethod]
        public void AlwaysPicksMostDerivedMethod()
        {
            // Fixed for with ints loaded and empty statement.
            var tree = Trees.ForFixed();

            var visitor = new SwapsIntsWithBoolsExtended();
            visitor.Visit((Node)tree); // cast prevents direct invocation of expr method.

            Assert.IsTrue(visitor.Replacements == 0);
        }

        [TestMethod]
        public void ReplacingWithNullRemoves()
        {
            var blockTree = Trees.ForFixedForAndStatementBlock();
            var visitor = new RemoveAllBlocks();

            // Run on children only, as this is a block itself.
            visitor.VisitChildren(blockTree);
            Assert.IsTrue(blockTree.Children(allInTree: true).Count() == 10);
            
            // Run on self returns null.
            blockTree = visitor.Visit(blockTree);
            Assert.IsNull(blockTree);

            // Casting to (Node) to remove single dispatch should result in the same.
            Node blockTree2 = Trees.ForFixedForAndStatementBlock();
            blockTree2 = visitor.Visit(blockTree2);
            Assert.IsNull(blockTree2);

            // Test again with different visitor.
            var visitor2 = new RemoveAllStatementsExceptBlocksAndFor();
            blockTree = Trees.ForFixedForAndStatementBlock();

            Assert.IsTrue(blockTree.Children(allInTree: true).Count() == 14);

            // This should remove all statements, except both for's, double except their do statements.
            Node result = visitor2.Visit((Node) blockTree);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Children(allInTree: true).Count() == 9);
        }
    }
}
