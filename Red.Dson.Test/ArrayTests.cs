
using MongoDB.Bson;
using NUnit.Framework;
using Shouldly;

namespace Red.Dson.Test
{
    [TestFixture]
    public class ArrayTests
    {
        [Test]
        public void Set_AcceptsRegularArray()
        {
            var root = Dson.New();
            root.a = new[] {1, 2};

            Assert.That((BsonDocument) root, Is.EqualTo(BsonDocument.Parse(@"{a:[1,2]}")));
        }

        [Test]
        public void Set_AcceptsBsonArray()
        {
            var root = Dson.New();
            root.a = new BsonArray(new[] {1, 2});

            Assert.That((BsonDocument) root, Is.EqualTo(BsonDocument.Parse(@"{a:[1,2]}")));
        }


        [Test]
        public void Get_ArrayIndexer()
        {
            var root = Dson.New(BsonDocument.Parse(@"{a:[0,1,2,3]}"));

            BsonInt32 x = root.a[1];
            x.AsInt32.ShouldBe(1);    // casting needs to be on separate lines.
        }

        [Test]
        public void Add()
        {
            var dson = Dson.New(new[] {1, 2});
            dson.Add(3);

            BsonArray array = dson;
            CollectionAssert.AreEquivalent(new BsonArray(new[] {1, 2, 3}), array);
        }

        [Test]
        public void AddRange()
        {
            var dson = Dson.New(new[] { 1, 2 });
            dson.AddRange(new[]{ 3, 4});
            dson.AddRange(5, 6);

            BsonArray array = dson;
            CollectionAssert.AreEquivalent(new BsonArray(new[] { 1, 2, 3, 4, 5, 6}), array);
        }

        [Test]
        public void Clear()
        {
            var dson = Dson.New(new[] { 1, 2 });
            dson.Clear();

            BsonArray array = dson;
            array.Count.ShouldBe(0);
        }


        [Test]
        public void Contains()
        {
            var dson = Dson.New(new[] { 1, 2 });

            Assert.IsTrue(dson.Contains(1));
            Assert.IsTrue(dson.Contains(BsonValue.Create(1)));
        }


        [Test]
        public void Remove()
        {
            var dson = Dson.New(new[] { 1, 2, 3});
            dson.Remove(1);
            dson.Remove(BsonValue.Create(2));

            BsonArray array = dson;
            CollectionAssert.AreEquivalent(new BsonArray(new[] { 3 }), array);
        }

    }
}
