using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;
using CH.Bson;
using DynamicBson.Extensions;
using MongoDB.Bson;
using Red.Dson.Behaviors.Interface;

namespace Red.Dson.Behaviors.Implementation
{
    internal class DocumentBehavior : IDocumentBehavior
    {
        public BsonType BsonType
        {
            get
            {
                return BsonType.Document;
            }
        }
        
        object IBehavior.InvokeMember(Dson dson, string name, params object[] args)
        {
            Contract.Requires(dson.Bson is BsonDocument);

            var doc = (BsonDocument) dson.Bson;

            switch (name)
            {
                case "SelectValue":
                    var @default = args.Length >= 2
                                       ? args[1].AsBson()
                                       : null;

                    return args.IfInitial<string>( (s,a) => doc.SelectValue(s,@default));

                    
            }

            // This part is is for member setting.  E.g. dson.first_name("John").last_name("Mason");
            if (args.Length == 1)
            {
                doc[name] = BsonValue.Create(args[0]);
            }
            else
            {
                doc[name] = new BsonArray(args);
            }

            return dson; // return doc again to allow chaining.
        }


        bool IBehavior.TryGetMember(Dson dson, string name, out object result)
        {
            Contract.Requires(dson.Bson is BsonDocument);

            var bsonDoc = (BsonDocument) dson.Bson;

            if (bsonDoc.Contains(name))
            {
                result = new Dson(() => bsonDoc[name], b => bsonDoc[name] = b);
                return true;
            }

            // if member not found
            bsonDoc[name] = new BsonDocument();
            result = new Dson(() => bsonDoc[name], b => bsonDoc[name] = b);
            return true;
        }


        bool IBehavior.TrySetMember(Dson dson, string name, object value)
        {
            Contract.Requires(dson.Bson is BsonDocument);

            var bsonDoc = (BsonDocument) dson.Bson;
            var v = IsAnonymousType(value.GetType())
                        ? value.ToBsonDocument()
                        : value;
            bsonDoc[name] = BsonValue.Create(v);

            return true;
        }

        private static bool IsAnonymousType(Type type)
        {
            Contract.Requires(type != null);

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                       && type.IsGenericType && type.Name.Contains("AnonymousType")
                       && (type.Name.StartsWith("<>", StringComparison.OrdinalIgnoreCase) ||
                           type.Name.StartsWith("VB$", StringComparison.OrdinalIgnoreCase))
                       && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

    }
}
