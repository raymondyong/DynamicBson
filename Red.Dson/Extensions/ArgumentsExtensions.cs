using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicBson.Extensions
{
    public static class ArgumentsExtensions
    {
       
        public static object IfInitial<T>(this IEnumerable<object> args, Func<T, IEnumerable<object>, object> func, object @default = null)
        {
            if (args.Any())
            {
                var key = args.First();
                if (key.GetType() == typeof(T))
                    return func((T)key, args.Skip(1));
            }
            return @default;
        }

        public static object IfSingleArgument(this IEnumerable<object> args, Func<object, object> func)
        {
            return args.Count() == 1
                ? func(args.Single())
                : false;
        }
    }
}
