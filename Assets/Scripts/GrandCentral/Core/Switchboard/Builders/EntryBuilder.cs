using System;
using GrandCentral.Switchboard.Criterion;
using GrandCentral.Switchboard.Mutations;

namespace GrandCentral.Switchboard.Builders
{
    public class EntryBuilder : IEntryBuilderCriteria, IEntryBuilderMutations
    {
        internal IEntry Entry { get; set; }

        public IEntryBuilderCriteria New(string name)
        {
            Entry = new Entry(name);
            return this;
        }

        public IEntryBuilderMutations SetPayload(string payload)
        {
            ((Entry)Entry).Payload = payload;
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

        public IEntryBuilderMutations FactMutate<T>(string fact, string key, Func<T, T> setter)
        {
            Entry.StateMutations.Add(new ProcMutation<T>(fact, key, setter));
            return this;
        }

        public IEntryBuilderMutations FactSet<T>(string fact, string key, T val)
        {
            Entry.StateMutations.Add(new SetMutation<T>(fact, key, val));
            return this;
        }

        public IEntryBuilderMutations SetNextEntry(string name)
        {
            ((Entry)Entry).NextEntry = name;
            return this;
        }
    }
}