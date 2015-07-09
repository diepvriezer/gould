using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uva.Gould.Graphing.Elements
{
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

            Name = GraphStyles.GetCleanTypeName(type);
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

        public bool IsInheritance
        {
            get { return Name == null; }
        }

        public override string ToString()
        {
            return IsInheritance
                ? Source + " is base class of " + Target
                : Source + "." + Name + " is of " + Target;
        }
    }
}
