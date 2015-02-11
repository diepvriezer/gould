using System;

namespace Uva.Gould.Phases
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class PhaseAttribute : Attribute
    {
        public PhaseAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}
