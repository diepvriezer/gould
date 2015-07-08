public class LinkedNode<T> : Node
    where T : Node
{
    [Child] public T Node { get; set; }
    [Child] public LinkedNode<T> Next { get; set; }

    // Retrieves the last linked list node in the chain.
    public LinkedNode<T> Last();
    // Inserts node directly adjecent to this one.
    public void InsertAfter(T node);
    // Inserts a collection of nodes directly adjecent to this node.
    public void InsertAfter(IEnumerable<T> nodes);
    // Appends a node at the back of the linked list.
    public void Append(T node);
    // Appends a collection of nodes at the back of the linked list.
    public void Append(IEnumerable<T> nodes);
    // Enumerates all stored nodes in the list, excluding the containers.
    public IEnumerable<T> NodesInLinkedList();
}

static class Extensions
{
	// Convers a collection of nodes of type T to a chain of linked list nodes.
	public static LinkedNode<T> ToLinkedNode<T>(this IEnumerable<T> collection);
}