using System.Collections.Generic;

namespace Uva.Gould.Traversals
{
    public static class PropertyIter
    {
        /// <summary>
        /// Returns an enumeration over all direct children of node T.
        /// </summary>
        public static IEnumerable<object> Children<T>(this T node, bool includeSelf = false)
        {
            if (includeSelf)
                yield return node;

            foreach (var prop in node.GetType().GetOrderedChildProps())
            {
                var child = prop.GetValue(node);
                if (child != null)
                    yield return child;
            }
        }

        /// <summary>
        /// Returns a recursive enumeration of all children, grandchildren, etc of node T.
        /// </summary>
        public static IEnumerable<object> AllChildren<T>(this T node, bool includeSelf = false)
        {
            if (includeSelf)
                yield return node;

            // Stack based approach to eliminate recursion.
            var stack = new Stack<object>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                foreach (var prop in current.GetType().GetOrderedChildProps())
                {
                    var child = prop.GetValue(current);
                    if (child != null)
                    {
                        yield return child;
                        stack.Push(child);
                    }
                }
            }
        }
    }
}
