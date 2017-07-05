using System.Collections.Generic;
using GrandCentral;
using GrandCentral.Criterion;
using GrandCentral.Mutations;

namespace GrandCentral
{
    public interface IEntry
    {
        string EventName { get; }

        string Name { get; }
        List<ICriterion> Criteria { get; }

        List<IMutation> BlackboardMutations { get; }

        int Length { get; }
        bool Check(Blackboard context, BlackboardsContainer BlackboardsContainer);
    }
}