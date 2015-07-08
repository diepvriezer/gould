using System;

namespace Uva.Gould.Traversals
{
    /// <summary>
    /// Abstract implementation of a visitor which uses properties for AST nodes.
    /// </summary>
    public abstract class PropertyVisitor : IVisitor
    {
        // Main entry point for traversals.
        public virtual T Visit<T>(T node)
        {
            return (T)Visit(node, typeof(T));
        }

        // Required overload to communicate property type.
        protected abstract object Visit(object node, Type propertyType);

        /// <summary>
        /// Initiates automatic traversal which transforms and assigns all child nodes.
        /// </summary>
        /// <param name="node">Node whom's children should be traversed</param>
        public virtual void VisitChildren<T>(T node)
        {
            if (Equals(node, default(T)))
                return;

            // Traverse child nodes using attributes and replace with result from visitor.
            var nodeType = node.GetType();
            foreach (var prop in nodeType.GetOrderedChildProps())
            {
                var value = prop.GetValue(node);
                if (value != null)
                {
                    value = Visit(value, prop.PropertyType);
                    prop.SetValue(node, value);
                }
            }
        }
    }
}
