using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class OldVisitorTests : OldVisitor
    {
        /// <summary>
        ///       0
        ///      / \
        ///     1   4
        ///    / \
        ///   2   3
        /// </summary>
        [TestMethod]
        public void EmptyVisitorReturns()
        {
            RemoveAllHandlers();

            var tree = new BiNodeTree1();
            Assert.IsTrue(tree.Children(allInTree: true).OfType<BiNode>().Sum(n => n.Id) == 10);
            tree = Visit(tree);
            Assert.IsTrue(tree.Children(allInTree: true).OfType<BiNode>().Sum(n => n.Id) == 10);
        }
        [TestMethod]
        public void ReplaceWithNull()
        {
            RemoveAllHandlers();

            ReplaceIf<BiNode>(node => node.Id == 1, node => null);
            var tree = new BiNodeTree1();
            Assert.IsTrue(tree.Children(allInTree: true).OfType<BiNode>().Sum(n => n.Id) == 10);
            tree = Visit(tree);
            Assert.IsTrue(tree.Children(allInTree: true).OfType<BiNode>().Sum(n => n.Id) == 4);
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
        [TestMethod]
        public void ContextExplained()
        {
            // We're going to add handlers on the subtyped fixture for A2 with and without
            // context. The easiest way to demonstrate context is handler hitcount.

            // Base tree is as above.
            var tree = new BiNodeTree2(); // cast to node
            int hits;
            
            // With viewing we can hit all nodes, or any matching subtype.
            RemoveAllHandlers();
            hits = 0;
            View<Node>(n => { hits++; VisitChildren(n); });
            Visit(tree);
            Assert.IsTrue(hits == 7);
            
            // Count BiNodeA2's, of which we have four (ID: 2, 3, 5, 6)
            RemoveAllHandlers();
            hits = 0;
            View<BiNodeA2>(node => hits++);
            Visit(tree);
            Assert.IsTrue(hits == 4);

            // Count BiNodeA2's which could be replaced by at least a BiNodeA1, of which
            // we have 3 (ID: 3, 5, 6)
            RemoveAllHandlers();
            hits = 0;
            View<BiNodeA2, BiNodeA1>(node => hits++);
            Visit(tree);
            Assert.IsTrue(hits == 3);

            // Count BiNodeA2's which could be replaced by any BiNode (ID: 5, 6)
            RemoveAllHandlers();
            hits = 0;
            View<BiNodeA2, BiNode>(node => hits++);
            Visit(tree);
            Assert.IsTrue(hits == 2);

            // Same goes for replacing; lets replace all A2's with A1's where possible (should 
            // be possible on ID 3, 5 and 6.
            RemoveAllHandlers();
            Replace<BiNodeA2, BiNodeA1>(a2 => new BiNodeA1(a2.Id));
            Visit(tree);
            Assert.IsTrue(tree.Children(allInTree: true).Count(n => n.GetType() == typeof(BiNodeA1)) == 3);
        }

        /// <summary>
        ///       0
        ///      / \
        ///     1   4
        ///    / \
        ///   2   3
        /// </summary>
        [TestMethod]
        public void ConditionalsExplained()
        {
            var tree = new BiNodeTree1();

            // You can add predicates to view or replace actions to filter nodes.
            RemoveAllHandlers();
            ViewIf<BiNode>(node => node.Id == 2, node => Assert.IsTrue(node.Id == 2));
            Visit(tree);

            // Count uneven numbered nodes.
            RemoveAllHandlers();
            int sum = 0;
            ViewIf<BiNode>(n => n.Id%2 == 1, n =>
            {
                sum++; 
                VisitChildren(n); // this is crucial. always visit children unless not intended.
                // calling Visit directly is also possible
                // Visit(n.Left);
                // Visit(n.Right);
            });
            Visit(tree);
            Assert.IsTrue(sum == 2);
        }
    }
}
