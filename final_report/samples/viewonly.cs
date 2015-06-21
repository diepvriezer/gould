public abstract class ViewOnlyVisitor
{
	public void Visit<T>(T node) where T : Node
	{
		if (node == null)
			return;
	
		// If the visitor has a public visit method for this object, invoke.
		var method = GetType().GetMethod("Visit", new[] {node.GetType()});
		if (method != null && method.ReturnType == typeof (void))
		{
			method.Invoke(this, new object[] {node});
		}
		// Otherwise, traverse children.
		else
		{
			VisitChildren(node);
		}
	}
	
	public void VisitChildren(Node node)
	{
		if (node == null)
			return;
	
		// Traverse child nodes using attributes and submit to visitor.
		foreach (var prop in node.ChildProperties)
		{
			var value = (Node) prop.GetValue(node);
			if (value != null)
			{
				Visit(value);
			}
		}
	}
}