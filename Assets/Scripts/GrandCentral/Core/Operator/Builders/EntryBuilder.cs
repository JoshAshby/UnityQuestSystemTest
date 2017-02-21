namespace GrandCentral
{
    namespace Operator
    {
        namespace Builders
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

                public IEntryBuilderCriteria AddCriteron(string fact, string key, string val)
                {
                    Entry.Criteria.Add(new MatchCriterion<string>(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddCriteron(string fact, string key, int val)
                {
                    Entry.Criteria.Add(new MatchCriterion<int>(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddCriteron(string fact, string key, bool val)
                {
                    Entry.Criteria.Add(new MatchCriterion<bool>(fact, key, val));
                    return this;
                }

                public IEntryBuilderCriteria AddRangeCriteron(string fact, string key, int low, int high)
                {
                    Entry.Criteria.Add(new IntRangeCriterion(fact, key, low, high));
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

                public IEntryBuilderMutations FactSet<T>(string fact, string key, T val)
                {
                    Entry.StateMutations.Add(new SetMutation<T>(fact, key, val));
                    return this;
                }

                public IEntryBuilderMutations FactSet(string fact, string key, string val)
                {
                    return FactSet<string>(fact, key, val);
                }

                public IEntryBuilderMutations FactSet(string fact, string key, int val)
                {
                    return FactSet<int>(fact, key, val);
                }

                public IEntryBuilderMutations FactSet(string fact, string key, bool val)
                {
                    return FactSet<bool>(fact, key, val);
                }

                public IEntryBuilderMutations FactToggle(string fact, string key)
                {
                    Entry.StateMutations.Add(new ToggleMutation(fact, key));
                    return this;
                }

                public IEntryBuilderMutations FactIncrement(string fact, string key, int val)
                {
                    Entry.StateMutations.Add(new IncrementMutation(fact, key, val));
                    return this;
                }

                public IEntryBuilderMutations FactDecrement(string fact, string key, int val)
                {
                    Entry.StateMutations.Add(new DecrementMutation(fact, key, val));
                    return this;
                }

                public IEntryBuilderMutations SetNextEntry(string name)
                {
                    ((Entry)Entry).NextEntry = name;
                    return this;
                }
            }
        }
    }
}