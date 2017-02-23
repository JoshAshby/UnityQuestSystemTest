using System.Collections.Generic;
using GrandCentral.FileCabinet;
using GrandCentral.Switchboard.Criterion;
using GrandCentral.Switchboard.Mutations;

namespace GrandCentral.Switchboard
{
    internal interface IEntry
    {
        string Name { get; }
        List<ICriterion> Criteria { get; }
        List<IStateMutation> StateMutations { get; }

        string Payload { get; }
        string NextEntry { get; }

        int Length { get; }
        bool Check(FactShard context);
    }
}