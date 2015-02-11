using System;
using System.Runtime.CompilerServices;

namespace Uva.Gould
{
    /// <summary>
    /// Denotes a property as a 'child' of a node, which means it will be traversed
    /// by the visitor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ChildAttribute : Attribute
    {
        private readonly int _order;

        // C# 4.5 only, see Microsoft.Bcl package for < 4.5
        public ChildAttribute([CallerLineNumber] int order = 0)
        {
            _order = order;
        }

        public int Order
        {
            get { return _order; }
        }
    }
}
