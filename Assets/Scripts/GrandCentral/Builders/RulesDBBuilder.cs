using System;
using System.Linq;
using GrandCentral.Criterion;
using GrandCentral.Mutations;

namespace GrandCentral.Builders
{
    public class RuleDBBuilder : IRuleDBEntryBuilder, IEntryBuilderCriteria, IEntryBuilderOnMatch, IEntryBuilderMutations
    {
        private RuleDatabase _database;
        private IEntry Entry;

        public void IRuleDBEntryBuilder()
        {
            _database = new RuleDatabase();
        }

        public IEntryBuilderCriteria AddEntry(string name)
        {
            Entry = new Entry(name);
            return this;
        }

        public IEntryBuilderCriteria AddCriteron<T>(string fact, string key, Func<T, bool> compare)
        {
            Entry.Criteria.Add(new ProcCriterion<T>(fact, key, compare));
            return this;
        }

        public IEntryBuilderCriteria AddCriteron<T>(string key, Func<T, bool> compare)
        {
            Entry.Criteria.Add(new ProcCriterion<T>(key, compare));
            return this;
        }

        public IEntryBuilderOnMatch OnMatch()
        {
            return this;
        }

        public IEntryBuilderMutations FactMutate<T>(string fact, string key, Func<T, T> setter)
        {
            Entry.StateMutations.Add(new ProcMutation<T>(fact, key, setter));
            return this;
        }

        public IEntryBuilderMutations FactSet<T>(string fact, string key, T val)
        {
            Entry.StateMutations.Add(new ProcMutation<T>(fact, key, (T currentVal) => val));
            return this;
        }

        public IRuleDBEntryBuilder ReturnPayload(string payload)
        {
            ((Entry)Entry).Payload = payload;
            _database.Entries.Add(Entry);

            Entry = null;

            return this;
        }

        public RuleDatabase Finalize()
        {
            _database.Entries = _database.Entries.OrderByDescending(x => x.Length).ToList();

            return _database;
        }
    }
}