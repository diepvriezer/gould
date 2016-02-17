using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uva.Gould.Support;
using Uva.Gould.Tests.Fixtures;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class LinkedNodeTests
    {
        private LinkedNode<IdNode> GetTestChain()
        {
            return new LinkedNode<IdNode>()
            {
                Node = new IdNode(0),
                Next = new LinkedNode<IdNode>()
                {
                    Node = new IdNode(1),
                    Next = new LinkedNode<IdNode>()
                    {
                        Node = null,
                        Next = new LinkedNode<IdNode>()
                        {
                            Node = new IdNode(2),
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
            Assert.IsTrue(ids.SequenceEqual(new [] { 0, 1, 2 }));
        }

        [TestMethod]
        public void InsertInChain()
        {
            var head = GetTestChain();
            var next = head.Next;
            next.InsertAfter(new IdNode(3));

            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 3, 2 }));
        }
        
        [TestMethod]
        public void AppendToChain()
        {
            var head = GetTestChain();
            head.Append(new IdNode(3));
            head.Append(new IdNode(4));
            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new [] { 0, 1, 2, 3, 4 }));
        }

        [TestMethod]
        public void EasyConstructionWithExtensions()
        {
            var collection = new[] {new IdNode(0), new IdNode(1), new IdNode(2), new IdNode(3)};
            var head = collection.ToLinkedNode();

            var ids = head.NodesInLinkedList().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new [] { 0, 1, 2, 3 }));

            // Cast enclosing type, for instance when appending statements.
            var nodeHead = collection.ToLinkedNode<Node>();
            ids = nodeHead.Children(allInTree: true).OfType<IdNode>().Select(n => n.Id);
            Assert.IsTrue(ids.SequenceEqual(new[] { 0, 1, 2, 3 }));
        }
    }
}
