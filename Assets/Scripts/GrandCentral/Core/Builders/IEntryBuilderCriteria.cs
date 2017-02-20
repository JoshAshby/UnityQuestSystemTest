namespace GrandCentral
{
    namespace Builders
    {
        public interface IEntryBuilderCriteria
        {
            IEntryBuilderCriteria AddCriteron(string key, string val);
            IEntryBuilderCriteria AddCriteron(string key, int val);
            IEntryBuilderCriteria AddCriteron(string key, int low, int high);
        }
    }
}