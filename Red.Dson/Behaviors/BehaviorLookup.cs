using System.Collections.Generic;
using System.Linq;
using CH.IoC.Infrastructure.Wiring;
using MongoDB.Bson;
using Red.Dson.Behaviors.Implementation;
using Red.Dson.Behaviors.Interface;

namespace Red.Dson.Behaviors
{
    [Wire]
    internal class BehaviorLookup
    {
        private static readonly IDictionary<BsonType, IBehavior> DsonBehaviors =
            new IBehavior[]
                {
                    new DocumentBehavior(),
                    new ArrayBehavior()
                }.ToDictionary(
                    b => b.BsonType,
                    b => b
                );


        public static IBehavior Find(BsonType bsonType)
        {
            IBehavior behavior;

            return DsonBehaviors.TryGetValue(bsonType, out behavior) 
                ? behavior 
                : null;
        }
    }
}
