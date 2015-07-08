class Node { }
class Derived : Node { }
class A : Derived { }
class B : Derived { }

class SomeVisitor : GeneralizedVisitor
{
	public B Visit(A node)
	{
		return new B();
	}
}

var visitor = new SomeVisitor();
A a = new A();
a = visitor.Visit(a);    /* what happens? what should happen? */