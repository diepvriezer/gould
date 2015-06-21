using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Uva.Gould
{
    /// <summary>
    /// Various utility methods used by the library.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Like Type.ToString(), but shows generics propertly.
        /// </summary>
        public static string GetCleanName(this Type t)
        {
            string name = t.Name;
            if (t.IsGenericType)
            {
                name = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
                var args = t.GetGenericArguments();
                for (int i = 0; i < args.Length; i++)
                {
                    name += args[i].Name;
                    if (i != args.Length - 1)
                        name += ", ";
                }
                name += ">";
            }

            return name;
        }

        internal static readonly Dictionary<Type, IReadOnlyList<PropertyInfo>> CachedProperties = new Dictionary<Type, IReadOnlyList<PropertyInfo>>(); 
        
        /// <summary>
        /// Enumeration of the [Child] attributes on Node properties on a type.
        /// </summary>
        public static IReadOnlyList<PropertyInfo> GetChildNodeProperties(this Type t)
        {
            if (!CachedProperties.ContainsKey(t))
            {
                CachedProperties[t] = t
                    .GetProperties()
                    .Where(p => p.CustomAttributes.Any(att => att.AttributeType == typeof (ChildAttribute))
                                && (p.PropertyType == typeof (Node) || p.PropertyType.IsSubclassOf(typeof (Node))))
                    .OrderBy(p => p.GetCustomAttribute<ChildAttribute>().Order)
                    .ToList();
            }

            return CachedProperties[t];
        }

        /// <summary>
        /// Returns the description for the enum value, first tries [Description] then defaults to
        /// ToString().
        /// </summary>
        public static string GetDescription(this Enum en)
        {
            if (en == null)
                return null;

            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return en.ToString();
        }

        /// <summary>
        /// Strips all non-ascii characters from a string.
        /// </summary>
        public static string ToAscii(this string s)
        {
            return Encoding.ASCII.GetString(
                Encoding.Convert(
                    Encoding.UTF8,
                    Encoding.GetEncoding(
                        Encoding.ASCII.EncodingName,
                        new EncoderReplacementFallback(string.Empty),
                        new DecoderExceptionFallback()
                        ),
                    Encoding.UTF8.GetBytes(s)
                    )
                );
        }

        /// <summary>
        /// Converts a collection of nodes into a singly linked list. Returns the head of the list.
        /// Parent references are handled automatically.
        /// </summary>
        /// <typeparam name="T">Type of items in linked list and collection</typeparam>
        /// <param name="collection">Collection of nodes</param>
        /// <param name="parent">Parent of the head node</param>
        /// <returns>Head of the linked list node of type T</returns>
        public static LinkedNode<T> ToLinkedNode<T>(this IEnumerable<T> collection)
            where T : Node
        {
            return collection.ToLinkedNode<T, T>();
        }

        /// <summary>
        /// Converts a collection of nodes into a singly linked list. Returns the head of the list.
        /// Parent references are handled automatically.
        /// </summary>
        /// <typeparam name="T">Type of nodes in collection</typeparam>
        /// <typeparam name="TContext">Type of the linked list</typeparam>
        /// <param name="collection">Collection of nodes</param>
        /// <param name="parent">Parent of the head node</param>
        /// <returns>Head of the linked list node of type T</returns>
        public static LinkedNode<TContext> ToLinkedNode<TContext, T>(this IEnumerable<T> collection)
            where T : TContext
            where TContext : Node
        {
            if (collection == null)
                return null;

            var head = new LinkedNode<TContext>();
            foreach (var item in collection)
                head.Append(item);

            // Return only if n > 0.
            return head.Node != null || head.Next != null ? head : null;
        }
    }
}
