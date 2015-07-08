class Generalized<Root>
{
	T visit(T v) where T : Root
	{
		return Visit(v, typeof(T));
	}
	
	private T visit(T v, Type max) where T : Root
	{
		if ( v == null )
			return null;
			
		if  (/* this visitor has a public visit method for the runtime type of v */)
		and (/* the return type of this method is equal to or a subclass of max  */)
			return (T) invoke(method(v));
		else
			for (each field f of v, with type ft)
				v.f = Visit(v.f, ft);
	}
}