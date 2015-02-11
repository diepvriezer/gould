using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uva.Gould.Tests.Support
{
    [TestClass]
    public class TestBase
    {
        private List<Tuple<LoadResourceAttribute, MemberInfo>> _members;
        private Dictionary<MemberInfo, string> _lookup;

        private readonly Assembly _asm = Assembly.GetExecutingAssembly();
        private Type _t;

        [TestInitialize]
        public void PrepareEmbeddedResource()
        {
            // Initialize lookup and attrib list.
            if (_t == null) _t = GetType();
            if (_members == null)
            {
                _members = GetAttributes().ToList();
                _lookup = new Dictionary<MemberInfo, string>();
            }

            // Store embedded resources, as this method will be called on each test.
            foreach (var kv in _members)
            {
                var member = kv.Item2;

                // Read resource and store in lookup if not present.
                if (!_lookup.ContainsKey(member))
                {
                    var attrib = kv.Item1;
                    _lookup[member] = GetResource(attrib);
                }
                
                // Store resource in member field.
                SetMemberValue(member, _lookup[member]);
            }
        }

        private void SetMemberValue(MemberInfo member, string value)
        {
            var prop = member as PropertyInfo;
            if (prop != null)
            {
                prop.SetValue(this, value);
            }
            else
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    field.SetValue(this, value);
                }
                else
                {
                    throw new Exception("Unknown memberinfo!");
                }
            }
        }

        private string GetResource(LoadResourceAttribute attrib)
        {
            var asmStream = attrib.IsRelative
                ? _asm.GetManifestResourceStream(_t, attrib.Path)
                : _asm.GetManifestResourceStream(attrib.Path);

            if (asmStream != null)
            {
                using (var reader = new StreamReader(asmStream))
                    return reader.ReadToEnd();
            }
            else
            {
                throw new Exception("Resource " + attrib.Path + " not found, ensure it is marked as " +
                                    "an embedded resource and the case matches.");
            }
        }

        private IEnumerable<Tuple<LoadResourceAttribute, MemberInfo>> GetAttributes()
        {
            return _t
                .GetFields().Cast<MemberInfo>()
                .Union(_t.GetProperties())
                .Where(m => m.GetCustomAttribute<LoadResourceAttribute>() != null)
                .Select(m => new Tuple<LoadResourceAttribute, MemberInfo>(m.GetCustomAttribute<LoadResourceAttribute>(), m));
        }
    }
}
