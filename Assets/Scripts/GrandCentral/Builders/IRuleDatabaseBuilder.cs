using System;

namespace GrandCentral.Builders
{
    public interface IRuleDatabaseBuilder
    {
        IRuleDatabaseEntryBuilderCriteria AddEntry(string name);
        RuleDatabase Build();
    }

    public interface IRuleDatabaseEntryBuilderCriteria
    {
        IRuleDatabaseEntryBuilderCriteria AddCriteron<T>(string fact, string key, Func<T, bool> proc);
        IRuleDatabaseEntryBuilderCriteria AddCriteron<T>(string key, Func<T, bool> proc);

        IRuleDatabaseEntryBuilderOnMatch OnMatch();

        IRuleDatabaseBuilder ReturnPayload(string name);
    }

    public interface IRuleDatabaseEntryBuilderOnMatch
    {
        IRuleDatabaseEntryBuilderMutations MutateFact<T>(string fact, string key, Func<T, T> proc);
        IRuleDatabaseEntryBuilderMutations SetFact<T>(string fact, string key, T val);

        IRuleDatabaseBuilder ReturnPayload(string name);
    }

    public interface IRuleDatabaseEntryBuilderMutations
    {
        IRuleDatabaseEntryBuilderMutations MutateFact<T>(string fact, string key, Func<T, T> proc);
        IRuleDatabaseEntryBuilderMutations SetFact<T>(string fact, string key, T val);

        IRuleDatabaseBuilder ReturnPayload(string name);
    }
}