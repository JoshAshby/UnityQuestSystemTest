using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    class Query
    {
        public Dictionary<string, int> Context;

        public Query()
        {
            Context = new Dictionary<string, int>();
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
            _query.Context.Add(key, val.GetHashCode());
            return this;
        }

        public IQueryBuilderAdd Add(string key, int val)
        {
            _query.Context.Add(key, val);
            return this;
        }

        public Query Query { get { return _query; } }
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
        public List<Criteron> Criteria;
        public object Payload;

        public Entry()
        {
            Criteria = new List<Criteron>();
        }

        public int Length
        {
            get { return Criteria.Count; }
        }

        public bool Check(Query query)
        {
            return Criteria.All(x => x.Check(query.Context));
        }
    }

    class Criteron
    {
        protected int AccessKey;

        protected int UpperValue;
        protected int LowerValue;

        public Criteron(string key, string value)
        {
            AccessKey = key.GetHashCode();

            UpperValue = value.GetHashCode();
            LowerValue = value.GetHashCode();
        }

        public Criteron(string key, int value)
        {
            AccessKey = key.GetHashCode();

            UpperValue = value;
            LowerValue = value;
        }

        public Criteron(string key, int lower, int upper)
        {
            AccessKey = key.GetHashCode();

            UpperValue = upper;
            LowerValue = lower;
        }

        public bool Check(Dictionary<int, int> state)
        {
            if (!state.ContainsKey(AccessKey))
                return false;

            int value = state[AccessKey];

            return value >= LowerValue && UpperValue >= value;
        }
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
            _entry.Criteria.Add(new Criteron(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int val)
        {
            _entry.Criteria.Add(new Criteron(key, val));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron(string key, int low, int high)
        {
            _entry.Criteria.Add(new Criteron(key, low, high));
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

        public EntryBuilder AddEntry(object payload)
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
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == "wat");

            queryBuilder.New().Add("test2", 5);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test2", 6);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == "bat");

            queryBuilder.New().Add("test3", 5);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test3", 13);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New().Add("test3", 10);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == "mat");

            queryBuilder.New()
                .Add("test4", "robin")
                .Add("test5", 19)
                .Add("test6", 7);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == null);

            queryBuilder.New()
                .Add("test4", "robin")
                .Add("test5", 19)
                .Add("test6", 3);
            Debug.Assert((string) testPartition.Query(queryBuilder.Query) == "hat");
        }
    }
}