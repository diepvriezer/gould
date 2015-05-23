using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class ViewOnlyVisitorTests
    {
        public class IncrementIntValue : ViewOnlyVisitor
        {
            public void Visit(Int node)
            {
                node.Value++;
            }
        }

        public class CountExpressionsExceptInt : ViewOnlyVisitor
        {
            public int Count { get; set; }

            public void Visit(Expression node)
            {
                Count++;
                VisitChildren(node); // Traverse into children, otherwise execution stops directly.
            }

            public void Visit(Int node)
            {
                // Do nothing.
            }
        }

        [TestMethod]
        public void VisitsChildNodes()
        {
            // Binop with two ints both set to 0.
            var tree = Trees.IntegerBinOp();

            var visitor = new IncrementIntValue();
            visitor.Visit(tree);

            Assert.IsNotNull(tree.Left);
            Assert.IsNotNull(tree.Right);
            Assert.IsInstanceOfType(tree.Left, typeof (Int));
            Assert.IsInstanceOfType(tree.Right, typeof (Int));
            Assert.IsTrue(((Int)tree.Left).Value == 1 && ((Int)tree.Right).Value == 1);
        }

        [TestMethod]
        public void FindsLessDerivedMethods()
        {
            // Binop with an int and bool, visitor should count 1 expression, other one
            // will be caught by specific handler.
            var tree = Trees.IntegerBooleanBinOp();

            var visitor = new CountExpressionsExceptInt();
            visitor.Visit(tree);
            Assert.IsTrue(visitor.Count == 2);

            tree = Trees.NestedBinOps();
            visitor.Count = 0;
            visitor.Visit(tree);
            Assert.IsTrue(visitor.Count == 5);
        }

        [TestMethod]
        public void FindsChildren()
        {
            var tree = Trees.NestedBinOps();
            var visitor = new CountExpressionsExceptInt();
            visitor.VisitChildren(tree);
            Assert.IsTrue(visitor.Count == 4); // doesn't include top node.
        }
    }
}
