using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Tests.Fixtures;
using Uva.Gould.Traversals;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class SllTests
    {
        private Sll<IncNode> GetTestChain()
        {
            IncNode.StartId = 0;
            return new Sll<IncNode>()
            {
                Node = new IncNode(),
                Next = new Sll<IncNode>()
                {
                    Node = new IncNode(),
                    Next = new Sll<IncNode>()
                    {
                        Node = null,
                        Next = new Sll<IncNode>()
                        {
                            Node = new IncNode(),
                            Next = null
                        }
                    }
                }
            };
        }


        [TestMethod]
        public void LastSkipsNullNodes()
        {
            var last = GetTestChain().Last();
            Assert.IsNotNull(last);
            Assert.IsNotNull(last.Node);
            Assert.IsTrue(last.Node.Id == 2);
        }

        [TestMethod]
        public void EnumeratorSkipsNullNodes()
        {
            var head = GetTestChain();
            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 2 }));
        }

        [TestMethod]
        public void InsertInChain()
        {
            var head = GetTestChain();
            var next = head.Next;
            next.InsertAfter(new IncNode());

            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 3, 2 }));
        }

        [TestMethod]
        public void AppendToChain()
        {
            var head = GetTestChain();
            head.Append(new IncNode());
            head.Append(new IncNode());
            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void EasyConstructionWithExtensions()
        {
            IncNode.StartId = 0;
            var collection = new[] { new IncNode(), new IncNode(), new IncNode(), new IncNode() };
            var head = collection.ToSll();

            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 2, 3 }));

            // Cast enclosing type, for instance when appending statements.
            var nodeHead = collection.ToSll<object>();
            ids = nodeHead.AllChildren().OfType<IncNode>().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 2, 3 }));
        }
    }
}
