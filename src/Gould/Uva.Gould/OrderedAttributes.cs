using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Uva.Gould
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TravMethodAttribute : OrderedAttribute
    {
        public TravMethodAttribute([CallerLineNumber] int order = 0) : base(order) { }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ChildAttribute : OrderedAttribute
    {
        public ChildAttribute([CallerLineNumber] int order = 0) : base(order) { }
    }

    // Shortcuts for OrderedAttributeCache.
    public static class TypeExtensions
    {
        public static IReadOnlyList<PropertyInfo> GetOrderedChildProps(this Type type)
        {
            return OrderedAttributeCache<ChildAttribute, PropertyInfo>.GetOrderedAttributes(type);
        }

        public static IReadOnlyList<MethodInfo> GetOrderedTravMethods(this Type type)
        {
            return OrderedAttributeCache<TravMethodAttribute, MethodInfo>.GetOrderedAttributes(type);
        }
    }
}
