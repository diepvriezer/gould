class ConcreteNode : Node
{
	[Child] public OtherNode Child1 { get; set; }
	[Child] public OtherNode Child2 { get; set; }
	
	public string Attribute { get; set; }
}