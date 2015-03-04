using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace StackExchange.Redis.Extensions.Data.Tests
{
    [TestClass]
    public class PropertyKeyGeneratorTests
    {
        [TestMethod]
        public void PropertyKeyGeneratorTest()
        {
            var c = new ClassForKey { Email = "john.smith@awesome" };
            var kg = new PropertyKeyGenerator("Email");
            var k = kg.Generate(c);

            Assert.AreEqual("email:john.smith@awesome", k.ToString());
        }

        [TestMethod]
        public void GenerateTest()
        {
            var c = new ClassForKey { Id = 1000 };
            var kg = new PropertyKeyGenerator("Id");
            var k = kg.Generate(c);

            Assert.AreEqual("classforkeyid:1000", k.ToString());
        }

        [TestMethod]
        public void GenerateTest2()
        {
            var c = new ClassForKey { Uid = 1000 };
            var kg = new PropertyKeyGenerator("Uid");
            var k = kg.Generate(c);

            Assert.AreEqual("uid:1000", k.ToString());
        }

        private class ClassForKey
        {
            public string Email { get; set; }

            public int Id { get; set; }

            public int Uid { get; set; }
        }
    }

    [TestClass]
    public class IndexGeneratorTests
    {
        [TestMethod]
        public void KeyTest()
        {
            var c = new TestClass() { Id = 1000, Email = "test@foo.local" };
            var kg = new IndexKeyGenerator(new IdentityKeyGenerator());
            var keys = kg.Generate(c);

            var k1 = keys.First();
            Assert.AreEqual("id:1000:email", k1.PropertyName);
            Assert.AreEqual(c.Email.ToLowerInvariant(), k1.PropertyValue);

            var k2 = keys.Last();
            Assert.AreEqual("email:test@foo.local:id", k2.PropertyName);
            Assert.AreEqual(c.Id.ToString(), k2.PropertyValue);
        }

        private class TestClass
        {
            [Key]
            public int Id { get; set; }

            [Index]
            public string Email { get; set; }
        }
    }
}