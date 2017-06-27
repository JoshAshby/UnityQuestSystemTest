using System.Collections.Generic;
using GrandCentral.Facts;
using GrandCentral.Switchboard.Criterion;
using GrandCentral.Switchboard.Mutations;

namespace GrandCentral.Switchboard
{
    public interface IEntry
    {
        string Name { get; }
        List<ICriterion> Criteria { get; }

        List<IStateMutation> StateMutations { get; }
        string NextEntry { get; }

        string Payload { get; }

        int Length { get; }
        bool Check(FactDictionary context);
    }
}