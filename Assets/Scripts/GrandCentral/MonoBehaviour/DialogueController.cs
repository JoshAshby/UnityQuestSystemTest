using GrandCentral.Operator;
using GrandCentral.Operator.Builders;

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

            builder.AddEntry("seen-robin")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 0)
                .SetPayload("one-robin")
                .FactIncrement("player", "cylinders-seen", 1)
                .SetNextEntry("seen-one-robin-01");

            builder.AddEntry("seen-one-robin-01")
                .SetPayload("seen-one-robin");

            builder.AddEntry("seen-robin")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 1)
                .SetPayload("two-robins")
                .FactIncrement("player", "cylinders-seen", 1)
                .SetNextEntry("seen-two-robins-01");

            builder.AddEntry("seen-two-robins-01")
                .SetPayload("seen-two-robins");

            builder.AddEntry("seen-robin")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 2)
                .SetPayload("three-robins")
                .FactIncrement("player", "cylinders-seen", 1)
                .SetNextEntry("seen-three-robins-01");

            builder.AddEntry("seen-three-robins-01")
                .SetPayload("seen-three-robins");

            builder.AddEntry("seen-robin")
                .AddCriteron("query", "bird", "robin")
                .AddGteCriteron("player", "cylinders-seen", 3)
                .SetPayload("many-robins")
                .SetNextEntry("seen-many-robins-01");

            builder.AddEntry("seen-robin")
                .AddCriteron("query", "bird", "robin")
                .AddCriteron("player", "cylinders-seen", 3)
                .AddCriteron("query", "speaker", "protag")
                .SetPayload("many-robins")
                .FactIncrement("player", "cylinders-seen", 1)
                .SetNextEntry("seen-many-robins-02");

            builder.AddEntry("seen-many-robins-01")
                .SetPayload("seen-many-robins");

            builder.AddEntry("seen-many-robins-02")
                .SetPayload("seen-many-robins-twice");

            Rules.Add("bird-cylinders", builder.Finalize());
        }
    }
}