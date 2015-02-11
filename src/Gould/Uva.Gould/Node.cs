using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uva.Gould
{
    /// <summary>
    /// Node in the strongly typed AST. Child nodes are traversed by adding the
    /// [Child] attribute on public properties of type Node.
    /// </summary>
    public abstract class Node
    {
        private List<PropertyInfo> _properties;

        /// <summary>
        /// Properties which have the [Child] attribute, ordered by line of declaration.
        /// </summary>
        public IReadOnlyList<PropertyInfo> ChildProperties
        {
            get { return _properties ?? (_properties = GetType().GetChildNodeProperties().ToList()); }
        }

        /// <summary>
        /// Source code information for this node.
        /// </summary>
        public SourceInfo SourceInfo { get; set; }

        /// <summary>
        /// Enumeration of non-null child nodes. Optionally enumerates all child nodes
        /// of child nodes (recursively).
        /// </summary>
        public IEnumerable<Node> Children(bool allInTree = false, bool includeSelf = false)
        {
            if (includeSelf)
                yield return this;

            if (allInTree)
            {
                var s = new Stack<Node>();
                s.Push(this);
                while (s.Any())
                {
                    var current = s.Pop();
                    foreach (var prop in current.ChildProperties)
                    {
                        var child = (Node)prop.GetValue(current);
                        if (child != null)
                        {
                            yield return child;
                            s.Push(child);
                        }
                    }
                }
            }
            else
            {
                foreach (var prop in ChildProperties)
                {
                    var child = (Node)prop.GetValue(this);
                    if (child != null)
                        yield return child;
                }
            }
        }
    }
}
