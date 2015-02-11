using System;

namespace Uva.Gould.Tests.Support
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class LoadResourceAttribute : Attribute
    {
        public LoadResourceAttribute(string path, bool isRelative = true)
        {
            if (path == null) throw new ArgumentNullException("path");

            _path = path;
            IsRelative = isRelative;
        }

        private string _path;
        public string Path { get { return _path; } }

        public bool IsRelative { get; set; }
    }
}
