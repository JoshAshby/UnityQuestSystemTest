using System.Collections.Generic;
using GrandCentral;
using GrandCentral.Criterion;
using GrandCentral.Mutations;

namespace GrandCentral
{
    public interface IEntry
    {
        string Name { get; }
        List<ICriterion> Criteria { get; }

        List<IStateMutation> StateMutations { get; }

        string Payload { get; }

        int Length { get; }
        bool Check(FactDictionary context, FactDatabase FactDatabase);
    }
}