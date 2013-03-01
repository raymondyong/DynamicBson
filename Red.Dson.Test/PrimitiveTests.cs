using System;
using DynamicBson;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    public class PrimitiveTests
    {
        [Test]
        public void AbleToAccessAPrimitivesMembers()
        {
            var root = Dson.New();
            root.i = 100;
            root.date = new DateTime(2000,1,1);

            // see if BsonInt32's Value property is available.
            Assert.AreEqual(100, root.i.AsInt32);
            // see if BsonDateTime's ToLocalTime() is available.
            Assert.AreEqual(new DateTime(2000, 1, 1), root.date.AsAsDateTime);
        }
    }
}
