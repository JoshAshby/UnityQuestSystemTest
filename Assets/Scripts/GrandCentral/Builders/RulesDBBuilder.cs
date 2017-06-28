using System.Linq;

namespace GrandCentral.Builders
{
    public class RuleDBBuilder : IRuleDBEntryBuilder
    {
        private ContextAwareRuleDatabase _database;

        public IRuleDBEntryBuilder New()
        {
            _database = new ContextAwareRuleDatabase();
            return this;
        }

        public IEntryBuilderCriteria AddEntry(string name)
        {
            EntryBuilder entryBuilder = new EntryBuilder();
            entryBuilder.New(name);

            _database.Entries.Add(entryBuilder.Entry);

            return entryBuilder;
        }

        public ContextAwareRuleDatabase Finalize()
        {
            _database.Entries = _database.Entries.OrderByDescending(x => x.Length).ToList();

            return _database;
        }
    }
}