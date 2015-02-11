using System;
using System.Collections.Generic;
using System.Linq;

namespace Uva.Gould.Tests.Support
{
    public class TreeComparer
    {
        /// <summary>
        ///     Checks if two Node hierarchies are the equally typed. If a null child
        ///     is found in one tree, it must also be null in the other.
        /// </summary>
        public static bool AreEqual(Node first, Node second)
        {
            var stack = new Stack<Tuple<Node, Node>>();

            stack.Push(new Tuple<Node, Node>(first, second));
            while (stack.Any())
            {
                var s = stack.Pop();
                Node a = s.Item1;
                Node b = s.Item2;

                if (a == null && b == null)
                    continue;
                if (a == null || b == null)
                    return false;
                
                int n = a.ChildProperties.Count;
                if (n != b.ChildProperties.Count)
                    return false;

                for (int i = 0; i < n; i++)
                {
                    var propA = a.ChildProperties[i];
                    var propB = b.ChildProperties[i];

                    var ctxA = propA.PropertyType;
                    var ctxB = propB.PropertyType;
                    if (!(ctxA.IsAssignableFrom(ctxB) && ctxB.IsAssignableFrom(ctxA)))
                        return false;

                    var childA = (Node)propA.GetValue(a);
                    var childB = (Node)propB.GetValue(b);
                    if (childA == null && childB == null)
                        continue;
                    if (childA == null || childB == null)
                        return false;
                    if (childA.GetType() != childB.GetType())
                        return false;

                    stack.Push(new Tuple<Node, Node>(childA, childB));
                }
            }
            return true;
        }
    }
}
