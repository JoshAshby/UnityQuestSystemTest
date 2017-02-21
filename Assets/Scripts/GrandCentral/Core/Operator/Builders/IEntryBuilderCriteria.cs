namespace GrandCentral
{
    namespace Operator
    {
        namespace Builders
        {
            public interface IEntryBuilderCriteria
            {
                IEntryBuilderCriteria AddCriteron(string fact, string key, string val);
                IEntryBuilderCriteria AddCriteron(string fact, string key, int val);
                IEntryBuilderCriteria AddCriteron(string fact, string key, bool val);

                IEntryBuilderCriteria AddRangeCriteron(string fact, string key, int low, int high);

                IEntryBuilderCriteria AddGteCriteron(string fact, string key, int val);
                IEntryBuilderCriteria AddLteCriteron(string fact, string key, int val);

                IEntryBuilderCriteria AddGtCriteron(string fact, string key, int val);
                IEntryBuilderCriteria AddLtCriteron(string fact, string key, int val);

                IEntryBuilderMutations SetPayload(string payload);
            }

            public interface IEntryBuilderMutations
            {
                IEntryBuilderMutations FactSet(string fact, string key, string val);
                IEntryBuilderMutations FactSet(string fact, string key, int val);
                IEntryBuilderMutations FactSet(string fact, string key, bool val);

                IEntryBuilderMutations FactSet<T>(string fact, string key, T val);

                IEntryBuilderMutations FactToggle(string fact, string key);

                IEntryBuilderMutations FactIncrement(string fact, string key, int val);
                IEntryBuilderMutations FactDecrement(string fact, string key, int val);

                IEntryBuilderMutations SetNextEntry(string name);
            }
        }
    }
}