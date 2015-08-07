public abstract class Walkabout
{
	public void Visit(Object v)
	{
		if ( v != null )
			if (/* this has a public visit method for the class of v */)
				this.Visit(v);
			else
				for (each field f of v)
					this.Visit(v.f);
	}
}