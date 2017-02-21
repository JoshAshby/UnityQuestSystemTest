using GrandCentral.Operator;
using GrandCentral.Operator.Builders;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Dialogue Controller", true)]
    public class DialogueController : Singleton<DialogueController>
    {
        public Rules Rules { get; private set; }

        private void Awake()
        {
            Rules = new Rules();

            RulesShardBuilder builder = new RulesShardBuilder();

            builder.New();

            builder.AddEntry("one-robin")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 0)
                .SetPayload("one-robin")
                .SetNextEntry("seen-one-robin");

            builder.AddEntry("seen-one-robin")
                .SetPayload("seen-one-robin");

            builder.AddEntry("two-robins")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 1)
                .SetPayload("two-robins")
                .SetNextEntry("seen-two-robins");

            builder.AddEntry("seen-two-robins")
                .SetPayload("seen-two-robins");

            builder.AddEntry("three-robins")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 2)
                .SetPayload("three-robins")
                .SetNextEntry("seen-three-robins");
            
            builder.AddEntry("seen-three-robins")
                .SetPayload("seen-three-robins");

            builder.AddEntry("lots-of-robins")
                .AddCriteron("query", "bird", "robin")
                .AddGteCriteron("player", "cylinders-seen", 3)
                .SetPayload("many-robins")
                .SetNextEntry("seen-many-robins");

            builder.AddEntry("seen-many-robins")
                .SetPayload("seen-many-robins");

            Rules.Add("bird-cylinders", builder.Finalize());
        }
    }
}