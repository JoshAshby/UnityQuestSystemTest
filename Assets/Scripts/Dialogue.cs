using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
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

    public interface IGenericData
    {
        Type Type { get; }
        object Value { get; }

        IGenericData<TResult> OfType<TResult>();
    }

    public interface IGenericData<T>
    {
        T Value { get; set; }
    }

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

    public interface IEntryBuilderCriteria
    {
        IEntryBuilderCriteria AddCriteron(string key, string val);
        IEntryBuilderCriteria AddCriteron(string key, int val);
        IEntryBuilderCriteria AddCriteron(string key, int low, int high);
    }

    public class EntryBuilder : IEntryBuilderCriteria
    {
        internal Entry Entry { get; set; }

        public EntryBuilder New()
        {
            Entry = new Entry();
            return this;
        }

        public IEntryBuilderCriteria SetPayload(object payload)
        {
            Entry.Payload = payload;
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, string val)
        {
            Entry.Criteria.Add(new MatchCriteron<string>(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int val)
        {
            Entry.Criteria.Add(new MatchCriteron<int>(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int low, int high)
        {
            Entry.Criteria.Add(new IntRangeCriteron(key, low, high));
            return this;
        }
    }

    public interface IRulesShardBuilderEntry
    {
        IRulesShardBuilderEntry New();
    }

    public class RulesShardBuilder : IRulesShardBuilderEntry
    {
        private RulesShard _database;

        public IRulesShardBuilderEntry New()
        {
            _database = new RulesShard();
            return this;
        }

        public IEntryBuilderCriteria AddEntry(object payload)
        {
            EntryBuilder entryBuilder = new EntryBuilder();
            entryBuilder.New().SetPayload(payload);

            _database.Entries.Add(entryBuilder.Entry);

            return entryBuilder;
        }

        public RulesShard Finalize()
        {
            _database.Entries = _database.Entries.OrderBy(x => x.Length).ToList();

            return _database;
        }
    }

    public class Loader
    {
        static public void Build()
        {
            RulesShardBuilder databaseBuilder = new RulesShardBuilder();

            databaseBuilder.New();
            databaseBuilder.AddEntry("wat").AddCriteron("test1", "alpha");
            databaseBuilder.AddEntry("bat").AddCriteron("test2", 6);
            databaseBuilder.AddEntry("mat").AddCriteron("test3", 9, 12);
            databaseBuilder.AddEntry("hat")
                .AddCriteron("test4", "robin")
                .AddCriteron("test5", 19)
                .AddCriteron("test6", 0, 6);

            RulesShard testPartition = databaseBuilder.Finalize();

            Debug.Assert(Query.From(testPartition).Where("test1", "beta").SelectAs<string>() == null);
            Debug.Assert(Query.From(testPartition).Where("test1", "alpha").SelectAs<string>() == "wat");

            Debug.Assert(Query.From(testPartition).Where("test2", 5).SelectAs<string>() == null);
            Debug.Assert(Query.From(testPartition).Where("test2", 6).SelectAs<string>() == "bat");

            Debug.Assert(Query.From(testPartition).Where("test3", 5).SelectAs<string>() == null);
            Debug.Assert(Query.From(testPartition).Where("test3", 13).SelectAs<string>() == null);
            Debug.Assert(Query.From(testPartition).Where("test3", 10).SelectAs<string>() == "mat");

            string res1 = Query.From(testPartition)
                .Where("test4", "robin")
                .Where("test5", 19)
                .Where("test6", 7)
                .SelectAs<string>();
            Debug.Assert(res1 == null);

            string res2 = Query.From(testPartition)
                .Where("test4", "robin")
                .Where("test5", 19)
                .Where("test6", 3)
                .SelectAs<string>();
            Debug.Assert(res2 == "hat");
        }
    }
}