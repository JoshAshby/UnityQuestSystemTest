using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Dialogue
{
    class Query
    {
        public Dictionary<int, int> Context;

        public Query()
        {
            Context = new Dictionary<int, int>();
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
            _query.Context.Add(key.GetHashCode(), val.GetHashCode());
            return this;
        }

        public IQueryBuilderAdd Add(string key, int val)
        {
            _query.Context.Add(key.GetHashCode(), val);
            return this;
        }

        public Query Query { get { return _query; } }
    }

    class DatabasePartition
    {
        protected List<Entry> Entries;

        public DatabasePartition()
        {
            Entries = new List<Entry>();
        }

        public DatabasePartition AddEntry(Entry entry)
        {
            Entries.Add(entry);
            return this;
        }

        public void Optimize()
        {
            Entries = Entries.OrderBy(x => x.Length).ToList();
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
        protected List<Criteron> Criteria;
        public object Payload;

        public Entry()
        {
            Criteria = new List<Criteron>();
        }

        public Entry AddCriteron(string key, string value)
        {
            Criteria.Add(new Criteron(key, value));
            return this;
        }

        public Entry AddCriteron(string key, int value)
        {
            Criteria.Add(new Criteron(key, value));
            return this;
        }

        public Entry AddCriteron(string key, int lower, int upper)
        {
            Criteria.Add(new Criteron(key, lower, upper));
            return this;
        }

        public Entry SetPayload(object payload)
        {
            Payload = payload;
            return this;
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

    class Loader
    {
        static public void Build()
        {
            DatabasePartition testPartition = new DatabasePartition();

            testPartition.AddEntry(
                new Entry()
                    .AddCriteron("test1", "alpha")
                    .SetPayload("wat")
            ).AddEntry(
                new Entry()
                    .AddCriteron("test2", 6)
                    .SetPayload("bat")
            ).AddEntry(
                new Entry()
                    .AddCriteron("test3", 9, 12)
                    .SetPayload("mat")
            ).AddEntry(
                new Entry()
                    .AddCriteron("test4", "robin")
                    .AddCriteron("test5", 19)
                    .AddCriteron("test6", 0, 6)
                    .SetPayload("hat")
            ).Optimize();

            QueryBuilder queryBuilder = new QueryBuilder();

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