using System;
using System.Linq;
using GrandCentral.Criterion;
using GrandCentral.Mutations;

namespace GrandCentral.Builders
{
    public class RuleDatabaseBuilder : IRuleDatabaseBuilder, IRuleDatabaseEntryBuilderCriteria, IRuleDatabaseEntryBuilderOnMatch, IRuleDatabaseEntryBuilderMutations
    {
        private RuleDatabase _database;
        private IEntry Entry;

        public RuleDatabaseBuilder()
        {
            _database = new RuleDatabase();
        }

        public IRuleDatabaseEntryBuilderCriteria AddEntry(string name)
        {
            Entry = new Entry(name);
            return this;
        }

        public IRuleDatabaseEntryBuilderCriteria AddCriteron<T>(string fact, string key, Func<T, bool> compare)
        {
            Entry.Criteria.Add(new ProcCriterion<T>(fact, key, compare));
            return this;
        }

        public IRuleDatabaseEntryBuilderCriteria AddCriteron<T>(string key, Func<T, bool> compare)
        {
            Entry.Criteria.Add(new ProcCriterion<T>(key, compare));
            return this;
        }

        public IRuleDatabaseEntryBuilderOnMatch OnMatch()
        {
            return this;
        }

        public IRuleDatabaseEntryBuilderMutations MutateFact<T>(string fact, string key, Func<T, T> setter)
        {
            Entry.StateMutations.Add(new ProcMutation<T>(fact, key, setter));
            return this;
        }

        public IRuleDatabaseEntryBuilderMutations SetFact<T>(string fact, string key, T val)
        {
            Entry.StateMutations.Add(new ProcMutation<T>(fact, key, (T currentVal) => val));
            return this;
        }

        public IRuleDatabaseBuilder ReturnPayload(string payload)
        {
            ((Entry)Entry).Payload = payload;
            _database.Entries.Add(Entry);

            Entry = null;

            return this;
        }

        public RuleDatabase Build()
        {
            _database.Entries = _database.Entries.OrderByDescending(x => x.Length).ToList();

            return _database;
        }
    }
}