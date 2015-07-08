using System;
using System.Collections.Generic;
using System.Reflection;
using Uva.Gould.Properties;

namespace Uva.Gould.Traversals
{
    /// <summary>
    /// Visitor which looks for publically defined Visit methods, return type being either
    /// void (which is equal to returning the same node) or some other type.
    /// </summary>
    public class MethodVisitor : PropertyVisitor
    {
        protected override object Visit(object node, Type propertyType)
        {
            // If this visitor has a public method for this class, with convertible return type, invoke.
            var t = node.GetType();
            var method = GetCachedVisitMethod(t);
            if (method != null)
            {
                if (method.ReturnType == typeof (void) && propertyType.IsAssignableFrom(t))
                {
                    method.Invoke(this, new object[] {node});
                    return node;
                }
                if (propertyType.IsAssignableFrom(method.ReturnType))
                {
                    return method.Invoke(this, new object[] { node });
                }
            }
            
            VisitChildren(node);

            return node;
        }

        // Static lookup table holding the applicable visitor method or null.
        private static Dictionary<Type, Dictionary<Type, MethodInfo>> cachedMethods = new Dictionary<Type, Dictionary<Type, MethodInfo>>();
        private Type visitorType;
        private MethodInfo GetCachedVisitMethod(Type nodeType)
        {
            if (visitorType == null)
                visitorType = GetType();

            if (!cachedMethods.ContainsKey(visitorType))
                cachedMethods.Add(visitorType, new Dictionary<Type, MethodInfo>());

            if (!cachedMethods[visitorType].ContainsKey(nodeType))
                cachedMethods[visitorType][nodeType] = visitorType.GetMethod(Settings.Default.VisitorMethodName, new[] {nodeType});

            return cachedMethods[visitorType][nodeType];
        }
    }
}
