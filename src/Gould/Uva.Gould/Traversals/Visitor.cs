using System;

namespace Uva.Gould.Traversals
{
    public abstract class Visitor
    {
        public T Visit<T>(T node)
            where T : Node
        {
            return Visit(node, typeof (T));
        }

        private T Visit<T>(T node, Type maxUpcast)
            where T : Node
        {
            if (node == null)
                return null;

            // If this visitor has a public method for this class, with convertible return type, invoke.
            var method = GetType().GetMethod("Visit", new[] {node.GetType()});
            if (method != null 
                && (method.ReturnType == maxUpcast || method.ReturnType.IsSubclassOf(maxUpcast)))
            {
                return (T) method.Invoke(this, new object[] {node});
            }
            // Otherwise, traverse children.
            else
            {
                VisitChildren(node);
            }

            return node;
        }

        public void VisitChildren(Node node)
        {
            if (node == null)
                return;

            // Traverse child nodes using attributes and replace with result from visitor.
            foreach (var prop in node.ChildProperties)
            {
                var value = (Node) prop.GetValue(node);
                if (value != null)
                {
                    value = Visit(value, prop.PropertyType);
                    prop.SetValue(node, value);
                }
            }
        }
    }
}
