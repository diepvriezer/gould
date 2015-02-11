using System.Collections.Generic;

namespace Uva.Gould
{
    /// <summary>
    /// Generic linked list node which stores an item and a reference
    /// to the next linked list node. Not named LinkedListNode because
    /// of the collision with System.Collections.
    /// </summary>
    /// <typeparam name="T">Type of items in list</typeparam>
    public class LinkedNode<T> : Node
        where T : Node
    {
        [Child] public T Node { get; set; }
        [Child] public LinkedNode<T> Next { get; set; }

        /// <summary>
        /// Retrieves the last linked list node in the chain.
        /// </summary>
        public LinkedNode<T> Last()
        {
            var n = this;
            while (n.Next != null)
                n = n.Next;

            return n;
        }
        
        /// <summary>
        /// Inserts node directly next to this one.
        /// </summary>
        public void InsertAfter(T node)
        {
            var newNext = new LinkedNode<T>()
            {
                Next = Next,
                Node = node
            };
            Next = newNext;
        }

        /// <summary>
        /// Inserts a collection of nodes directly next to this node.
        /// </summary>
        /// <param name="nodes"></param>
        public void InsertAfter(IEnumerable<T> nodes)
        {
            var head = nodes.ToLinkedNode();
            if (head == null)
                return;

            head.Last().Next = Next;
            Next = head;
        }

        /// <summary>
        /// Appends a node at the back of the linked list.
        /// </summary>
        public void Append(T node)
        {
            var last = Last();
            last.InsertAfter(node);
        }

        public void Append(IEnumerable<T> nodes)
        {
            var last = Last();
            last.InsertAfter(nodes);
        }

        /// <summary>
        /// Enumerates all stored nodes in the list, excludes the container.
        /// </summary>
        public IEnumerable<T> NodesInLinkedList()
        {
            var head = this;
            while (head != null)
            {
                if (head.Node != null)
                    yield return head.Node;

                head = head.Next;
            }
        }
    }
}
