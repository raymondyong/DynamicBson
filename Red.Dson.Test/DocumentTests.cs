using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Options;
using NUnit.Framework;

namespace Red.Dson.Test
{
    [TestFixture]
    public class DocumentTests
    {

        [Test]
        public void Set_PrimitiveTypes()
        {
            var root = Dson.New("first_name", "sarah");

            root["last_name"] = "johnson";
            root.age = 22;
            root.member_since = new DateTime(2000, 1, 1);
            root.followers = 22222L;
            root.referrals(new[] {100, 200, 300});

            Assert.AreEqual(
                new
                    {
                        first_name = "sarah",
                        last_name = "johnson",
                        age = 22,
                        member_since = new DateTime(2000, 1, 1),
                        followers = 22222L,
                        referrals = new[] {100, 200, 300}
                    }.ToBsonDocument(),
                root.AsBsonDocument);
        }


        [Test]
        public void Set_AcceptsBsonDocument()
        {
            var root = Dson.New();
            root.a = new BsonDocument("b", 100);

            Assert.That(root.a.AsBsonDocument, Is.EqualTo(BsonDocument.Parse(@"{b:100}")));
        }


        [Test]
        public void CreateFromAnonymousObject()
        {
            var root = Dson.New(
                new
                    {
                        first_name = "sarah",
                        last_name = "johnson",
                        age = 22,
                        member_since = new DateTime(2000, 1, 1),
                        followers = 22222L,
                        referrals = new[] {100, 200, 300}
                    });

            Assert.AreEqual(
                new
                    {
                        first_name = "sarah",
                        last_name = "johnson",
                        age = 22,
                        member_since = new DateTime(2000, 1, 1),
                        followers = 22222L,
                        referrals = new[] {100, 200, 300}
                    }.ToBsonDocument(),
                root.AsBsonDocument);
        }


        [Test]
        public void Set_NestedChainedCreation()
        {
            // mixture of sub-document and array
            var root = Dson.New();
            root.a.b[2].c = "hello world";

            Assert.That(root.AsBsonDocument, Is.EqualTo(BsonDocument.Parse(@"{a:{b:[{},{},{c:""hello world""}]}}")));
        }


        [Test]
        public void Set_CanSwitchType()
        {
            var bson = new BsonDocument
                {
                    {"doc", new BsonDocument()}
                };

            var dson = Dson.New(bson);
            dson.doc = "string";

            Assert.That(bson, Is.EqualTo(BsonDocument.Parse(@"{doc:""string""}")));
        }



        [Test]
        public void TryConvert_CastToBsonDocument()
        {
            // Dson holds a reference to Bson.
            var bsonDoc = new BsonDocument("a", 1);
            var dson = Dson.New(bsonDoc);

            
            BsonDocument bsonDoc1 = dson;
            var bsonDoc2 = dson.AsBsonDocument;
            var bsonDoc3 = (BsonDocument)dson.AsBsonDocument;
            
            Assert.AreEqual(bsonDoc.GetHashCode(), bsonDoc1.GetHashCode());
            Assert.AreEqual(bsonDoc.GetHashCode(), bsonDoc2.GetHashCode());
            Assert.AreEqual(bsonDoc.GetHashCode(), bsonDoc3.GetHashCode());
        }


        [Test]
        public void TryConvert_CanConvertBsonArray()
        {
            var bsonArray = new BsonArray(new[] {1, 2});
            var dson = Dson.New(bsonArray);

            BsonArray bsonArray1 = dson;
            var bsonArray2 = dson.AsBsonArray;

            Assert.AreEqual(bsonArray1.GetHashCode(), dson.GetHashCode());
            Assert.AreEqual(typeof (BsonArray), bsonArray1.GetType());
            Assert.AreEqual(bsonArray2.GetHashCode(), dson.GetHashCode());
            Assert.AreEqual(typeof (BsonArray), bsonArray2.GetType());
        }

        [Test]
        public void Overlay_WorksWithBsonDocument()
        {
            var root = Dson.New(new {a = 1});
            var withBson = new BsonDocument("b", 2);
            root.Overlay(withBson);

            Assert.That(root.AsBsonDocument, Is.EqualTo(BsonDocument.Parse(@"{a:1, b:2}")));
        }


        [Test]
        public void Overlay_WorksWithDson()
        {
            var root = Dson.New(new {a = 1});
            var withDson = Dson.New(new BsonDocument("b", 2));

            root.Overlay(withDson);

            Assert.That(root.AsBsonDocument, Is.EqualTo(BsonDocument.Parse(@"{a:1, b:2}")));
        }


        [Test]
        public void SelectValue_ReturnsValueWithoutChangingDocument()
        {
            // This is to check for a value without modifying the doc.  (as oppose to AvDocument)
            var root = Dson.New();
            root.a.b[2].c = "Hello World";

            Assert.IsNull(root.SelectValue("zzz"));
            Assert.That(root.AsBsonDocument, 
                        Is.EqualTo(BsonDocument.Parse(@"{a:{b:[{},{},{c:""Hello World""}]}}")));
        }


        [Test]
        public void Field_DifferentWaysToInitialize()
        {
            var root = Dson.New();
            root.num(100) // primitive
                .favorite_colors("blue", "red", "green") // array
                .favorite_food(new string[] {}) // empty array
                .first_name("ray")
                .last_name("yong")
                .ssn(11111); // chaining

            Assert.That(root.AsBsonDocument,
                        Is.EqualTo(
                            new
                                {
                                    num = 100,
                                    favorite_colors = new[] {"blue", "red", "green"},
                                    favorite_food = new string[] {},
                                    first_name = "ray",
                                    last_name = "yong",
                                    ssn = 11111
                                }.ToBsonDocument()
                            ));
        }

        [Test]
        public void Field_AnonymousObjectIntializer()
        {
            var root = Dson.New();
            root.address =
                new
                    {
                        line1 = "111 Main St",
                        City = "Seattle",
                        State = "WA"
                    };

            Assert.That(root.address.AsBsonDocument,
                        Is.EqualTo(
                            new
                                {
                                    line1 = "111 Main St",
                                    City = "Seattle",
                                    State = "WA"
                                }.ToBsonDocument()
                            ));
        }


        [Test]
        public void IdTest()
        {
           var doc = Dson.New(
                new 
                    {
                        id = 1,
                        _id = 2
                    }.ToBsonDocument());

            BsonDocument b = doc;
            Console.WriteLine(doc);
        }

        [Test]
        public void Equal()
        {
            var doc = Dson.New();

            doc.a = 100;
            doc.b = 100;
            doc.c = 200;

            Assert.AreEqual(doc.a, doc.b);
            Assert.AreNotEqual(doc.a, doc.c);
        }

    }
}
