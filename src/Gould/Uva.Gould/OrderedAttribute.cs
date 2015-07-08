using System;

namespace Uva.Gould
{
    /// <summary>
    /// Base class for all ordered attributes.
    /// </summary>
    public abstract class OrderedAttribute : Attribute
    {
        protected OrderedAttribute(int order = 0)
        {
            this.order = order;
        }

        private readonly int order;
        public int Order { get { return order; } }
    }
}
