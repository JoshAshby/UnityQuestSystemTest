namespace GrandCentral
{
    namespace Operator
    {
        namespace Builders
        {
            public interface IEntryBuilderCriteria
            {
                IEntryBuilderCriteria SetPayload(string payload);

                // IEntryBuilderCriteria AddCriteron(string key, string val);
                // IEntryBuilderCriteria AddCriteron(string key, int val);

                // IEntryBuilderCriteria AddRangeCriteron(string key, int low, int high);

                // IEntryBuilderCriteria AddGteCriteron(string key, int val);
                // IEntryBuilderCriteria AddLteCriteron(string key, int val);

                // IEntryBuilderCriteria AddGtCriteron(string key, int val);
                // IEntryBuilderCriteria AddLtCriteron(string key, int val);

                IEntryBuilderCriteria AddCriteron(string fact, string key, string val);
                IEntryBuilderCriteria AddCriteron(string fact, string key, int val);

                IEntryBuilderCriteria AddRangeCriteron(string fact, string key, int low, int high);

                IEntryBuilderCriteria AddGteCriteron(string fact, string key, int val);
                IEntryBuilderCriteria AddLteCriteron(string fact, string key, int val);

                IEntryBuilderCriteria AddGtCriteron(string fact, string key, int val);
                IEntryBuilderCriteria AddLtCriteron(string fact, string key, int val);

                IEntryBuilderCriteria SetNextEntry(string name);
            }
        }
    }
}