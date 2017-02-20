namespace GrandCentral
{
    namespace Builders
    {
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
    }
}