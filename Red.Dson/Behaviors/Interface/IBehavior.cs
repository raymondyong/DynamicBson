using MongoDB.Bson;

namespace Red.Dson.Behaviors.Interface
{
    public interface IBehavior 
    {

        BsonType BsonType { get; }

        object InvokeMember(Dson dson, string name, params object[] args);
        bool  TryGetMember(Dson dson, string name, out object result);
        bool TrySetMember(Dson dson, string name, object value);
        //object GetIndex(Func<object> proceed, object self, IEnumerable<object> keys);
        //object SetIndex(Func<object> proceed, object self, IEnumerable<object> keys, object value);
        //object Convert(Func<object> proceed, object self, Type type, bool isExplicit);
        //object BinaryOperation(Func<object> proceed, object self, ExpressionType operation, object value);
        //object GetMembers(Func<object> proceed, object self, IDictionary<string, object> members);

        //object InvokeMemberMissing(Func<object> proceed, object self, string name, IEnumerable<object> args) ;
        //object GetMemberMissing(Func<object> proceed, object self, string name);
        //object SetMemberMissing(Func<object> proceed, object self, string name, object value);
        //object ConvertMissing(Func<object> proceed, object self, Type type, bool isExplicit);
    }
}
