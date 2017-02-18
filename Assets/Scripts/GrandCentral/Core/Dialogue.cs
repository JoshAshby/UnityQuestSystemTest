using System;
using System.Collections.Generic;
using System.Linq;

namespace GrandCentral
{
    public class GenericData<T> : IGenericData, IGenericData<T>
    {
        public GenericData(T val)
        {
            Value = val;
        }

        public Type Type { get { return typeof(T); } }
        private T _value = default(T);

        public object Value
        {
            get { return (object)_value; }
            set { _value = (T)value; }
        }

        object IGenericData.Value
        {
            get { return (object)Value; }
        }

        T IGenericData<T>.Value
        {
            get { return (T)Value; }
            set { Value = (object)value; }
        }

        public IGenericData<TResult> OfType<TResult>()
        {
            if (this is IGenericData<TResult>)
                return (IGenericData<TResult>)this;
            else
                return null;
        }
    }

    public class StateShard : Dictionary<string, IGenericData> { }

    public class RulesShard
    {
        internal List<Entry> Entries;

        public RulesShard()
        {
            Entries = new List<Entry>();
        }

        public object Query(Query query)
        {
            Entry entry = Entries.FirstOrDefault(x => x.Check(query));

            if (entry == null)
                return null;

            return entry.Payload;
        }
    }

    internal class Entry
    {
        internal List<ICriteron> Criteria;
        public object Payload;

        public Entry()
        {
            Criteria = new List<ICriteron>();
        }

        public int Length
        {
            get { return Criteria.Count; }
        }

        public bool Check(Query query)
        {
            return Criteria.All(x => x.Check(query));
        }
    }

    internal interface ICriteron
    {
        string AccessKey { get; }

        bool Check(Query query);
    }

    internal class MatchCriteron<T> : ICriteron
    {
        public string AccessKey { get; set; }

        private T _compareValue;

        public MatchCriteron(string key, T val)
        {
            AccessKey = key;

            _compareValue = val;
        }

        public bool Check(Query query)
        {
            if (!query.Context.ContainsKey(AccessKey))
                return false;

            IGenericData<T> val = query.Context[AccessKey].OfType<T>();

            if (val == null)
                return false;

            return val.Value.Equals(_compareValue);
        }
    }

    internal class IntRangeCriteron : ICriteron
    {
        public string AccessKey { get; set; }

        private int _lowCompareValue;
        private int _highCompareValue;

        public IntRangeCriteron(string key, int low, int high)
        {
            AccessKey = key;

            _lowCompareValue = low;
            _highCompareValue = high;
        }

        public bool Check(Query query)
        {
            if (!query.Context.ContainsKey(AccessKey))
                return false;

            IGenericData<int> val = query.Context[AccessKey].OfType<int>();

            if (val == null)
                return false;

            return _lowCompareValue <= val.Value && val.Value <= _highCompareValue;
        }
    }

    public class Query
    {
        public StateShard Context;

        protected RulesShard _databasePartition;
        protected Type _payloadType = typeof(object);

        public Query(RulesShard databasePartition)
        {
            Context = new StateShard();
            _databasePartition = databasePartition;
        }

        public static Query From(RulesShard databasePartition)
        {
            Query query = new Query(databasePartition);
            return query;
        }

        public Query Where(string key, string val)
        {
            Context.Add(key, new GenericData<string>(val));
            return this;
        }

        public Query Where(string key, int val)
        {
            Context.Add(key, new GenericData<int>(val));
            return this;
        }

        public object Select()
        {
            return _databasePartition.Query(this);
        }

        public TResult SelectAs<TResult>()
        {
            return (TResult)Select();
        }
    }

    public class State : Dictionary<string, StateShard> { }

    public class Rules : Dictionary<string, RulesShard>
    {
        public Query From(string key)
        {
            return Query.From(this[key]);
        }
    }
}