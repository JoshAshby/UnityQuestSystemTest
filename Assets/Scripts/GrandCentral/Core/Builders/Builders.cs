using System.Linq;

namespace GrandCentral
{
    namespace Builders
    {
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
    }
}