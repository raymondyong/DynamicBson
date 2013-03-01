using MongoDB.Bson;

namespace DynamicBson.Extensions
{
    public static class BsonExtensions
    {
        public static BsonValue AsBson(this object o )
        {
            var bson = o as BsonValue;
            if (bson != null)
                return bson;

            if (o.GetType().IsAnonymousType())
                return o.ToBsonDocument();

            return BsonValue.Create(o);
        }
    }
}
