using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Uva.Gould.Tests.Support.Tests
{
    [TestClass]
    public class TestBaseTests : TestBase
    {
        [LoadResource("TestFile.txt")]
        public string PropLoad { get; set; }
        [LoadResource("TestFile.txt")]
        public string FieldLoad;

        [TestMethod]
        public void FilesLoaded()
        {
            Assert.IsTrue(PropLoad != null);
            Assert.IsTrue(FieldLoad != null);
            Assert.IsTrue(PropLoad == FieldLoad);
            Assert.IsTrue(FieldLoad == "Test!");
        }
    }
}
