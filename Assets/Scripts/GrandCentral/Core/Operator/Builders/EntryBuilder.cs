namespace GrandCentral
{
    namespace Operator
    {
        namespace Builders
        {
            public class EntryBuilder : IEntryBuilderCriteria
            {
                internal IEntry Entry { get; set; }

                public EntryBuilder New(string name)
                {
                    Entry = new Entry(name);
                    return this;
                }

                public IEntryBuilderCriteria SetPayload(string payload)
                {
                    ((Entry)Entry).Payload = payload;
                    return this;
                }

                public IEntryBuilderCriteria AddCriteron(string fact, string key, string val)
                {
                    Entry.Criteria.Add(new MatchCriteron<string>(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new MatchCriteron<int>(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddRangeCriteron(string fact, string key, int low, int high)
                {
                    Entry.Criteria.Add(new IntRangeCriteron(fact, key, low, high));
                    return this;
                }

                public IEntryBuilderCriteria AddGteCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new IntGteCriteron(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddGtCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new IntGtCriteron(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddLteCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new IntLteCriteron(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddLtCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new IntLteCriteron(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria SetNextEntry(string name)
                {
                    ((Entry)Entry).NextEntry = name;
                    return this;
                }
            }
        }
    }
}