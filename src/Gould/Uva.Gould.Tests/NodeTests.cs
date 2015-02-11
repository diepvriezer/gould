using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class NodeTests
    {
        public class TestNode : Node
        {
            [Child] public Node A { get; set; }
            public Node B { get; set; }
            [Child] public Node C { get; set; }
        }

        [TestMethod]
        public void ReturnsOrderedChildProperties()
        {
            var testNode = new TestNode();
            var props = testNode.ChildProperties;
            Assert.IsNotNull(props);
            Assert.IsTrue(props.Count == 2);
            Assert.IsTrue(props.Select(p => p.Name).SequenceEqual(new[] {"A", "C"}));
        }

        [TestMethod]
        public void ChildEnumeratorReturnsNoNulls()
        {
            var innerTest = new TestNode();
            var testNode = new TestNode()
            {
                A = null,
                B = new TestNode(),
                C = innerTest
            };

            Assert.IsTrue(testNode.Children().Count() == 1);
            Assert.IsTrue(testNode.Children().First() == innerTest);
        }
    }
}
