using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Uva.Gould
{
    /// <summary>
    /// Cache which retrieves and orders member info based on the presence of an ordered attribute.
    /// </summary>
    public static class OrderedAttributeCache<TAttrib, TMemberInfo>
        where TAttrib : OrderedAttribute
        where TMemberInfo : MemberInfo
    {
        // Cache is unique for each type param combi.
        private static Dictionary<Type, IReadOnlyList<TMemberInfo>> cache = new Dictionary<Type, IReadOnlyList<TMemberInfo>>();

        public static IReadOnlyList<TMemberInfo> GetOrderedAttributes(Type type)
        {
            if (!cache.ContainsKey(type))
            {
                cache[type] = type
                    .GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                    .Select(m => new {m, attrib = m.GetCustomAttribute<TAttrib>()})
                    .Where(m => m.attrib != null)
                    .OrderBy(m => m.attrib.Order)
                    .Select(m => m.m)
                    .Cast<TMemberInfo>()
                    .ToList();
            }

            return cache[type];
        }
    }
}
