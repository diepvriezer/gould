class ConcreteNode : Node
{
	[Child] public OtherNode Child1 { get; set; }
	[Child] public OtherNode Child2 { get; set; }
	
	public string Attribute { get; set; }
}

abstract class Node
{
	// Cached list of traversible properties
	private List<PropertyInfo> _properties;
	
	// Returns a list of traversible children.
	public IReadOnlyList<PropertyInfo> ChildProperties
	{
		get
		{
			if (_properties == null)
			{
				_properties = GetType()
				 .GetProperties()
				 .Where(p => p.CustomAttributes.Any(att => att.AttributeType == typeof(ChildAttribute))
				 .ToList();
			}
			
			return _properties;
		}
	}
}

public sealed class ChildAttribute : Attribute
{
	// Attribute used only to mark properties
}