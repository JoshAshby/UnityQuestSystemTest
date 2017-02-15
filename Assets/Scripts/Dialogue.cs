using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
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

    class Query
    {
        public Dictionary<string, IGenericData> Context;

        public Query()
        {
            Context = new Dictionary<string, IGenericData>();
        }
    }

    class DatabasePartition
    {
        public List<Entry> Entries;

        public DatabasePartition()
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

    class Entry
    {
        public List<ICriteron> Criteria;
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

    interface ICriteron
    {
        string AccessKey { get; }

        bool Check(Query query);
    }

    class MatchCriteron<T> : ICriteron
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

    class IntRangeCriteron : ICriteron
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

    interface IQueryBuilderAdd
    {
        IQueryBuilderAdd Add(string key, string val);
        IQueryBuilderAdd Add(string key, int val);
    }

    class QueryBuilder : IQueryBuilderAdd
    {
        private Query _query;

        public IQueryBuilderAdd New()
        {
            _query = new Query();
            return this;
        }

        public IQueryBuilderAdd Add(string key, string val)
        {
            _query.Context.Add(key, new GenericData<string>(val));
            return this;
        }

        public IQueryBuilderAdd Add(string key, int val)
        {
            _query.Context.Add(key, new GenericData<int>(val));
            return this;
        }

        public Query Query { get { return _query; } }
    }

    interface IEntryBuilderCriteria
    {
        IEntryBuilderCriteria AddCriteron(string key, string val);
        IEntryBuilderCriteria AddCriteron(string key, int val);
        IEntryBuilderCriteria AddCriteron(string key, int low, int high);
    }

    class EntryBuilder : IEntryBuilderCriteria
    {
        private Entry _entry;

        public EntryBuilder New()
        {
            _entry = new Entry();
            return this;
        }

        public IEntryBuilderCriteria SetPayload(object payload)
        {
            _entry.Payload = payload;
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, string val)
        {
            _entry.Criteria.Add(new MatchCriteron<string>(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int val)
        {
            _entry.Criteria.Add(new MatchCriteron<int>(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int low, int high)
        {
            _entry.Criteria.Add(new IntRangeCriteron(key, low, high));
            return this;
        }
    }

    interface IDatabaseBuilderEntry
    {
        IDatabaseBuilderEntry New();
    }

    class DatabaseBuilder : IDatabaseBuilderEntry
    {
        private DatabasePartition _database;

        public IDatabaseBuilderEntry New()
        {
            _database = new DatabasePartition();
            return this;
        }

        public IEntryBuilderCriteria AddEntry(object payload)
        {
            EntryBuilder entryBuilder = new EntryBuilder();
            entryBuilder.New().SetPayload(payload);

            return entryBuilder;
        }

        public DatabasePartition Finalize()
        {
            _database.Entries = _database.Entries.OrderBy(x => x.Length).ToList();

            return _database;
        }
    }

    class Loader
    {
        static public void Build()
        {
            DatabaseBuilder databaseBuilder = new DatabaseBuilder();
            QueryBuilder queryBuilder = new QueryBuilder();

            databaseBuilder.New();
            databaseBuilder.AddEntry("wat").AddCriteron("test1", "alpha");
            databaseBuilder.AddEntry("bat").AddCriteron("test2", 6);
            databaseBuilder.AddEntry("mat").AddCriteron("test3", 9, 12);
            databaseBuilder.AddEntry("hat")
                .AddCriteron("test4", "robin")
                .AddCriteron("test5", 19)
                .AddCriteron("test6", 0, 6);

            DatabasePartition testPartition = databaseBuilder.Finalize();

            queryBuilder.New().Add("test1", "beta");
            Debug.Assert(testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test1", "alpha");
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == "wat");

            queryBuilder.New().Add("test2", 5);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test2", 6);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == "bat");

            queryBuilder.New().Add("test3", 5);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test3", 13);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test3", 10);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == "mat");

            queryBuilder.New()
                .Add("test4", "robin")
                .Add("test5", 19)
                .Add("test6", 7);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New()
                .Add("test4", "robin")
                .Add("test5", 19)
                .Add("test6", 3);
            Debug.Assert((string)testPartition.Query(queryBuilder.Query) == "hat");
        }
    }
}