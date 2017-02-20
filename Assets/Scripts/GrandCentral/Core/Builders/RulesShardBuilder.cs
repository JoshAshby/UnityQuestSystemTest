using System.Linq;

namespace GrandCentral
{
    namespace Builders
    {
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