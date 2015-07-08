using System.Collections.Generic;

namespace Uva.Gould
{
    /// <summary>
    /// Generic linked list node which stores an item and a reference
    /// to the next linked list node. Not named LinkedListNode because
    /// of the collision with System.Collections.
    /// </summary>
    /// <typeparam name="T">Type of items in list</typeparam>
    public class Sll<T> 
    {
        [Child] public T Node { get; set; }
        [Child] public Sll<T> Next { get; set; }

        /// <summary>
        /// Retrieves the last linked list node in the chain.
        /// </summary>
        public Sll<T> Last()
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
            var newNext = new Sll<T>()
            {
                Next = Next,
                Node = node
            };
            Next = newNext;
        }

        /// <summary>
        /// Inserts a collection of nodes directly next to this node.
        /// </summary>
        public void InsertAfter(IEnumerable<T> nodes)
        {
            var head = nodes.ToSll();
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

        /// <summary>
        /// Appends a collection of nodes at the back of the linked list.
        /// </summary>
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

    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts a collection of nodes into a singly linked list. Returns the head of the list.
        /// Parent references are handled automatically.
        /// </summary>
        /// <typeparam name="T">Type of items in linked list and collection</typeparam>
        /// <param name="collection">Collection of nodes</param>
        /// <param name="parent">Parent of the head node</param>
        /// <returns>Head of the linked list node of type T</returns>
        public static Sll<T> ToSll<T>(this IEnumerable<T> collection)
        {
            return collection.ToSll<T, T>();
        }

        /// <summary>
        /// Converts a collection of nodes into a singly linked list. Returns the head of the list.
        /// Parent references are handled automatically.
        /// </summary>
        /// <typeparam name="T">Type of nodes in collection</typeparam>
        /// <typeparam name="TContext">Type of the linked list</typeparam>
        /// <param name="collection">Collection of nodes</param>
        /// <returns>Head of the linked list node of type T</returns>
        public static Sll<TContext> ToSll<TContext, T>(this IEnumerable<T> collection)
            where T : TContext
        {
            if (collection == null)
                return null;

            var head = new Sll<TContext>();
            foreach (var item in collection)
                head.Append(item);

            // Return only if n > 0.
            return head.Node != null || head.Next != null ? head : null;
        }
    }
}
