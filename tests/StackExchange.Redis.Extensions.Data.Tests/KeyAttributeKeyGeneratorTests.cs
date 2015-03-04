using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StackExchange.Redis.Extensions.Data.Tests
{
    [TestClass]
    public class KeyAttributeKeyGeneratorTests
    {
        [TestMethod]
        public void KeyAttributeGenerateTest()
        {
            var test = new KeyClass { Uid = 1000 };
            var kg = new IdentityKeyGenerator();
            var key = kg.Generate(test);

            Assert.AreEqual("uid:1000", key.ToString());
        }

        [TestMethod]
        public void NoKeyAttributeGenerateTest()
        {
            var test = new UnkeyedClassWithId { Id = 1000 };
            var kg = new IdentityKeyGenerator();
            var key = kg.Generate(test);

            Assert.AreEqual("unkeyedclasswithidid:1000", key.ToString());
        }

        [TestMethod]
        public void NoKeyAttributeGenerateTest2()
        {
            var test = new UnkeyedClass { UnkeyedClassId = 1000 };
            var kg = new IdentityKeyGenerator();
            var key = kg.Generate(test);

            Assert.AreEqual("unkeyedclassid:1000", key.ToString());
        }

        private class KeyClass
        {
            [Key]
            public int Uid { get; set; }
        }

        private class UnkeyedClassWithId
        {
            public int Id { get; set; }
        }

        private class UnkeyedClass
        {
            public int UnkeyedClassId { get; set; }
        }
    }
}