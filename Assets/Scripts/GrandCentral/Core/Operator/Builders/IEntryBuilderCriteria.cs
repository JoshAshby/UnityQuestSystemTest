using System;

namespace GrandCentral
{
    namespace Operator
    {
        namespace Builders
        {
            public interface IEntryBuilderCriteria
            {
                IEntryBuilderCriteria AddCriteron<T>(string fact, string key, Func<T, bool> proc);

                IEntryBuilderMutations SetPayload(string payload);
            }

            public interface IEntryBuilderMutations
            {
                IEntryBuilderMutations FactMutate<T>(string fact, string key, Func<T, T> proc);

                IEntryBuilderMutations FactSet<T>(string fact, string key, T val);

                IEntryBuilderMutations SetNextEntry(string name);
            }
        }
    }
}