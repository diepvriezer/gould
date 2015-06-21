public abstract class Visitor
{
	public T Visit<T>(T node) where T : Node
	{
		return Visit(node, typeof (T));
	}
	
	private T Visit<T>(T node, Type maxUpcast) where T : Node
	{
		if (node != null)
		{
			// If this visitor has a public method for this class, with convertible return type, invoke.
			var nodeType = node.GetType();
			var method = this.GetType().GetMethod("Visit", new[] { nodeType });
			if (method != null && (method.ReturnType == maxUpcast || method.ReturnType.IsSubclassOf(maxUpcast)))
			{
				return (T) method.Invoke(this, new [] {node});
			}	
			// Otherwise traverse children.
			else
			{		
				VisitChildren(node);
			}
		}
		
		return node;
	}
	
	void VisitChildren(Node node)
	{
		foreach (var prop of node.ChildProperties)
		{
			Node value = (Node) prop.GetValue();
			if (value != null)
			{
				value = Visit(value, prop.PropertyType);
				prop.SetValue(value);
			}
		}
	}
}