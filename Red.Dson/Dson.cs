using System;
using System.Dynamic;
using CH.Bson;
using DynamicBson.Extensions;
using MongoDB.Bson;
using Red.Dson.Behaviors;

namespace Red.Dson
{
    public partial class Dson : DynamicObject
    {

        private readonly Func<BsonValue> _getBson;
        private readonly Action<BsonValue> _setBson;

        internal BsonValue Bson
        {
            get { return _getBson(); }
            set { _setBson(value); }

        }

        public BsonDocument AsBsonDocument
        {
            get { return Bson as BsonDocument; }
        }

        public BsonArray AsBsonArray
        {
            get { return Bson as BsonArray; }
        }

        public BsonType BsonType
        {
            get { return Bson.BsonType; }
        }

        /// <summary>
        /// Returns a Dson that wraps a Bson.  Changes to Dson affects the Bson.
        /// </summary>
        //public static dynamic Wrap(BsonValue bson)
        //{
        //    return new Dson(bson);
        //}

        /// <summary>
        /// Returns a new Dson given a Bson.  Changes to Dson does NOT affect the Bson.
        /// </summary>
        public static dynamic New(BsonValue bson)
        {
            return new Dson(bson);
        }

        public static dynamic New(string label, dynamic v)
        {
            return new Dson(label, v);
        }

        public static dynamic New()
        {
            return new Dson(new BsonDocument());
        }

        public static dynamic New(object o)
        {
            return new Dson(o.AsBson());
        }


        // Constructors
        internal Dson(string name, dynamic value)
            : this(new BsonDocument(name, value))
        { }

        internal Dson(object o)
            : this(BsonValue.Create(o))
        { }

        internal Dson(BsonValue bson)
            : this(() => bson, b => bson = b)
        { }

        internal Dson(Func<BsonValue> getBson, Action<BsonValue> setBson)
        {
            _getBson = getBson;
            _setBson = setBson;
        }

        public Dson this[int i]
        {
            get
            {
                TopItUp(i);
                return new Dson(Bson.AsBsonArray[i]);
            }
            set
            {
                TopItUp(i);
                Bson.AsBsonArray[i] = value.Bson;
            }
        }


        public BsonValue this[string name]
        {
            get { return ((BsonDocument)Bson)[name]; }
            set { ((BsonDocument)Bson)[name] = value; }
        }
     

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var behavior = BehaviorLookup.Find(Bson.BsonType);
            return behavior.TrySetMember(this, binder.Name, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var behavior = BehaviorLookup.Find(Bson.BsonType);

            if (behavior != null && behavior.TryGetMember(this, binder.Name, out result))
                return true;
            
            // fallback plan.
            dynamic d = Bson;
            var property = d.GetType().GetProperty(binder.Name);

            if (property == null)
            {
                result = null;
                return false;
            }

            result = property.GetValue(d, null);
            return true;    
        }


        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            
            var behavior = BehaviorLookup.Find(Bson.BsonType);

            if (behavior != null)
            {
                result = behavior.InvokeMember(this, binder.Name, args);
                return true;
            }

            // fallback plan.
            dynamic d = Bson;
            var methodInfo = d.GetType().GetMethod(binder.Name);

            if (methodInfo != null)
            {
                result = methodInfo.Invoke(d, args);
                return true;
            }

            return false;
        }
        
        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = Bson;
            return true;
        }


        public void Overlay(BsonDocument with)
        {
            var doc = Bson as BsonDocument;
            if (doc != null)
                doc.Overlay(with);
        }

        public void Overlay(Dson d)
        {
            if (d.BsonType == BsonType.Document)
            {
                Overlay(d.AsBsonDocument);
            }
        }


        public override bool Equals(Object obj)
        {
            var dson = obj as Dson;
            if (dson != null)
                return Bson.Equals(dson.Bson);
            return Bson.Equals(obj);
        }

        public bool Equals(Dson dson)
        {
            return Bson.Equals(dson.Bson);
        }

        public bool Equals(BsonValue bv)
        {
            return Equals(Bson, bv);
        }

        
        public static bool operator ==(Dson a, Dson b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (((object)a == null) || ((object)b == null))
            {
                return false;
            }

            return a.Bson == b.Bson;
        }


        public static bool operator !=(Dson a, Dson b)
        {
            if (a == null || b == null)
                return true;

            return a.Bson != b.Bson;

        }
        

        public override int GetHashCode()
        {
// ReSharper disable NonReadonlyFieldInGetHashCode
            return (Bson!= null ? Bson.GetHashCode() : 0);
// ReSharper restore NonReadonlyFieldInGetHashCode
        }

        private void TopItUp(int i)
        {
            // convention: overwrite existing _bson to new BsonArray if attempting to access via int index.
            if (Bson.BsonType != BsonType.Array)
            {
                Bson = new BsonArray();
            }

            var array = Bson.AsBsonArray;

            for (var elementsShortBy = i + 1 - array.Count
                 ; elementsShortBy > 0
                 ; elementsShortBy--)
            {
                array.Add(new BsonDocument());
            }
        }

    }
}
