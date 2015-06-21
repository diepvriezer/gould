private T Visit<T>(T node, Type maxUpcast)
	where T : Node
{
	if (node == null)
		return null;
	
	// If a handler exist for this class with convertible return type, invoke.
	var nodeType = node.GetType();
	foreach (var handle in _handlers)
	{
		// Test for matching type (or subtype).
		if (!handle.Type.IsAssignableFrom(nodeType))
			continue;
		
		// Test for fitness of transformation result.
		if (!maxUpcast.IsAssignableFrom(handle.MaxUpcast))
			continue;
		
		// Test for predicate.
		if (handle.Predicate != null)
		{
			bool result = (bool) handle.Predicate.DynamicInvoke(node);
			if (!result)
				continue;	
		}
		
		// Check if this is an action or function.
		if (handle.Function != null)
		{
			return (T) handle.Function.DynamicInvoke(node);
		}
		else
		{
			handle.Action.DynamicInvoke(node);
			return node;
		}
	}
	
	// If no handlers are found or if all fail, traverse children.
	VisitChildren(node);
	
	return node;
}

internal class Handler
{
	public Type Type { get; set; }
	public Type MaxUpcast { get; set; }
	
	public Delegate Function { get; set; }
	public Delegate Action { get; set; }
	public Delegate Predicate { get; set; }
}