class Walkabout
{
	void visit(Object v)
	{
		if ( v != null )
			if (/* this has a public visit method for the class of v */)
				this.visit(v);
			else
				for (each field f of v)
					this.visit(v.f);
	}
}