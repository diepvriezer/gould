using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QuickGraph;

namespace Uva.Gould.Graphing
{
    /// <summary>
    /// Can generate graphs of object trees and the AST.
    /// </summary>
    public class GraphGenerator
    {
        // Static set of all loaded classes.
        private static HashSet<TypeInfo> allClasses = null;

        public BidirectionalGraph<AstVertex, AstEdge> CreateAstGraph(Type root)
        {
            FillAllClasses();

            var vertices = new Dictionary<Type, AstVertex>();
            var edges = new HashSet<AstEdge>();

            var stack = new Stack<Type>();
            stack.Push(root);

            bool repeat;
            do
            {
                repeat = false;

                // Empty stack and process types.
                while (stack.Count > 0)
                {
                    var type = stack.Pop();
                    if (!vertices.ContainsKey(type))
                        vertices.Add(type, new AstVertex(type) { IsRoot = type == root });

                    var v = vertices[type];
                    if (v.Processed)
                        continue;

                    // If the basetype of this type is in the list, add an edge.
                    if (type.BaseType != null && vertices.ContainsKey(type.BaseType))
                    {
                        var bv = vertices[type.BaseType];
                        if (!bv.DerivedTypes.Contains(type))
                        {
                            bv.DerivedTypes.Add(type);
                            edges.Add(new AstEdge(bv, v));
                        }
                    }

                    v.Processed = true;

                    // Add an edge for every child property.
                    foreach (var prop in type.GetOrderedChildProps())
                    {
                        var propType = prop.PropertyType;

                        if (!vertices.ContainsKey(propType))
                            vertices.Add(propType, new AstVertex(propType));

                        var pv = vertices[propType];
                        if (!pv.Processed)
                            stack.Push(propType);

                        edges.Add(new AstEdge(v, pv, prop.Name));
                    }
                }

                // Find all classes which inherit from the dictionary keys; add those not found to the stack.
                var keys = new HashSet<Type>(vertices.Keys);
                foreach (var missing in allClasses.Where(c => c.BaseType != null && keys.Contains(c.BaseType)))
                {
                    var type = missing.AsType();
                    if (!vertices.ContainsKey(type))
                    {
                        keys.Add(type);
                        stack.Push(type);
                        repeat = true;
                    }
                }
            // Keep repeating until no new derived classes are found
            } while (repeat);


            var g = new BidirectionalGraph<AstVertex, AstEdge>();
            g.AddVertexRange(vertices.Values);
            g.AddEdgeRange(edges);

            return g;
        }

        private void FillAllClasses()
        {
            if (allClasses != null)
                return;

            allClasses = new HashSet<TypeInfo>(AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.DefinedTypes.Where(t => t.IsClass)));
        }

        private static string GetCleanTypeName(Type t)
        {
            string name = t.Name;
            if (t.IsGenericType)
            {
                name = 
                    t.Name.Substring(0, t.Name.IndexOf('`')) 
                    + "<" +  string.Join(", ", t.GetGenericArguments().Select(GetCleanTypeName)) 
                    + ">";
            }

            return name;
        }
    
        /// <summary>
        /// Vertexes represent class types, be it abstract, concrete generic or leaf.
        /// </summary>
        public class AstVertex
        {
            public AstVertex(Type type)
            {
                if (!type.IsClass)
                    throw new InvalidOperationException("Can only instantiate vertex from classes");

                IsAbstract = type.IsAbstract;
                DerivedTypes = new HashSet<Type>();
                Processed = false;

                Name = GetCleanTypeName(type);
                Type = type;
            }

            public bool IsRoot { get; set; }
            public bool Processed { get; set; }

            public Type Type { get; private set; }
            public bool IsAbstract { get; private set; }
            public HashSet<Type> DerivedTypes { get; private set; }
            public string Name { get; private set; }

            public override string ToString()
            {
                return Name;
            }
        }

        /// <summary>
        /// Edges represent both inheritance (Target -> Source) and named properties ([Source] Target.Name).
        /// </summary>
        public class AstEdge : IEdge<AstVertex>
        {
            public AstEdge(AstVertex source, AstVertex target, string name = null)
            {
                Source = source;
                Target = target;
                Name = name;
            }

            public AstVertex Source { get; private set; }
            public AstVertex Target { get; private set; }

            public string Name { get; set; }
            public bool IsInheritance { get { return Name == null; } }

            public override string ToString()
            {
                return IsInheritance
                    ? Source + " is base class of " + Target
                    : Source + "." + Name + " is of " + Target;
            }
        }
    }
}
