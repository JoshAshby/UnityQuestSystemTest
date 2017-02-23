using System.Collections.Generic;
using GrandCentral.Operator.Criterion;
using GrandCentral.Operator.Mutations;

namespace GrandCentral.Operator
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