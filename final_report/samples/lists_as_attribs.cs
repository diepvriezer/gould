class FunctionDefinition : Node 
{
	[Child] public List<VarDec> Variables { get; set; }
	[Child] public List<Statement> Statement { get; set; }
	[Child] public Expression Return { get; set; }
}