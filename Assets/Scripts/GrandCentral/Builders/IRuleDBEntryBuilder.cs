using System;

namespace GrandCentral.Builders
{
    public interface IRuleDBEntryBuilder
    {
        IEntryBuilderCriteria AddEntry(string name);
        RuleDatabase Finalize();
    }

    public interface IEntryBuilderCriteria
    {
        IEntryBuilderCriteria AddCriteron<T>(string fact, string key, Func<T, bool> proc);
        IEntryBuilderCriteria AddCriteron<T>(string key, Func<T, bool> proc);

        IEntryBuilderOnMatch OnMatch();

        IRuleDBEntryBuilder ReturnPayload(string name);
    }

    public interface IEntryBuilderOnMatch
    {
        IEntryBuilderMutations FactMutate<T>(string fact, string key, Func<T, T> proc);
        IEntryBuilderMutations FactSet<T>(string fact, string key, T val);

        IRuleDBEntryBuilder ReturnPayload(string name);
    }

    public interface IEntryBuilderMutations
    {
        IEntryBuilderMutations FactMutate<T>(string fact, string key, Func<T, T> proc);
        IEntryBuilderMutations FactSet<T>(string fact, string key, T val);

        IRuleDBEntryBuilder ReturnPayload(string name);
    }
}