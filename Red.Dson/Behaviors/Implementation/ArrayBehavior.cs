using System.Collections;
using System.Diagnostics.Contracts;
using System.Linq;
using CH.IoC.Infrastructure.Wiring;
using DynamicBson.Extensions;
using MongoDB.Bson;
using Red.Dson.Behaviors.Interface;

namespace Red.Dson.Behaviors.Implementation
{
    [Wire]
    public class ArrayBehavior : IArrayBehavior
    {
        public BsonType BsonType
        {
            get { return BsonType.Array; }
        }

        public object InvokeMember(Dson dson, string name, params object[] args)
        {
            Contract.Requires(dson.Bson is BsonArray);

            var array = (BsonArray) dson.Bson;
            switch (name)
            {
                case "AddRange":
                    if (args.Count() == 1 && args[0] is IEnumerable)
                        array.AddRange(args[0] as dynamic);
                    else
                        array.AddRange(args);
                    return dson;

                case "Add":
                    array.Add((dynamic)args.Single());
                    return array;

                case "Clear":
                    array.Clear();
                    return array; 

                case "Contains":
                    return args.IfSingleArgument(
                        arg => array.Contains(arg.AsBson()));

                case "Remove":
                    return args.IfSingleArgument(
                        arg => array.Remove(arg.AsBson()));

            }
            return null;
        }

        bool IBehavior.TrySetMember(Dson dson, string name, object value)
        {
            // arrays have no member to set.
            return false;
        }

        bool IBehavior.TryGetMember(Dson dson, string name, out object result)
        {
            // arrays have no member to get.
            result = null;
            return false;
        }


    }
}
