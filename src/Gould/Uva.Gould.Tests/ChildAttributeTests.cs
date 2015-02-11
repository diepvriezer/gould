using System;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uva.Gould.Tests
{
    [TestClass]
    public class ChildAttributeTests
    {
        public class OrderedProperties
        {
            [Child] public Object A { get; set; }
            [Child] public Object B { get; set; }
            [Child(5)] public Object C { get; set; }
        }

        [TestMethod]
        public void UsesCallerLine()
        {
            var props = typeof (OrderedProperties).GetProperties();
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                var child = prop.GetCustomAttribute<ChildAttribute>();

                // This is a bit risky, one import change and this fails.
                if (prop.Name == "A")
                    Assert.IsTrue(child.Order == 12);
                if (prop.Name == "B")
                    Assert.IsTrue(child.Order == 13);
                if (prop.Name == "C")
                    Assert.IsTrue(child.Order == 5);
            }
        }
    }
}
