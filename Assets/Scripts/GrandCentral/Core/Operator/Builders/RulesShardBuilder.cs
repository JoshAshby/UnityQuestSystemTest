using System.Linq;

namespace GrandCentral.Operator.Builders
{
    public class RulesShardBuilder : IRulesShardBuilderEntry
    {
        private RulesShard _database;

        public IRulesShardBuilderEntry New()
        {
            _database = new RulesShard();
            return this;
        }

        public IEntryBuilderCriteria AddEntry(string name)
        {
            EntryBuilder entryBuilder = new EntryBuilder();
            entryBuilder.New(name);

            _database.Entries.Add(entryBuilder.Entry);

            return entryBuilder;
        }

        public RulesShard Finalize()
        {
            _database.Entries = _database.Entries.OrderByDescending(x => x.Length).ToList();

            return _database;
        }
    }
}