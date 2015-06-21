abstract class Node
{
	..
	
	// Returns a list of traversible children.
	public IReadOnlyList<PropertyInfo> ChildProperties
	{
		get
		{
			if (_properties == null)
			{
				_properties = GetType()
				 .GetProperties()
				 .Where(p.PropertyType == typeof(Node) || p.PropertyType.IsSubclassOf(typeof(Node)))
				 .Where(p => p.CustomAttributes.Any(att => att.AttributeType == typeof(ChildAttribute))
				 .OrderBy(p => p.GetCustomAttribute<ChildAttribute>().Order)
				 .ToList();
			}
			
			return _properties;
		}
	}
}

public sealed class ChildAttribute : Attribute
{
	private readonly int _order;
	
	// C# 4.5 only, see Microsoft.Bcl package for < 4.5
	public ChildAttribute([CallerLineNumber] int order = 0)
	{
		_order = order;
	}
	
	public int Order
	{
		get { return _order; }
	}
}