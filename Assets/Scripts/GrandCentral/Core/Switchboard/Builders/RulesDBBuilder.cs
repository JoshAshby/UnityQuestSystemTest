using System.Linq;

namespace GrandCentral.Switchboard.Builders
{
    public class RuleDBBuilder : IRuleDBEntryBuilder
    {
        private RuleDB _database;

        public IRuleDBEntryBuilder New()
        {
            _database = new RuleDB();
            return this;
        }

        public IEntryBuilderCriteria AddEntry(string name)
        {
            EntryBuilder entryBuilder = new EntryBuilder();
            entryBuilder.New(name);

            _database.Entries.Add(entryBuilder.Entry);

            return entryBuilder;
        }

        public RuleDB Finalize()
        {
            _database.Entries = _database.Entries.OrderByDescending(x => x.Length).ToList();

            return _database;
        }
    }
}