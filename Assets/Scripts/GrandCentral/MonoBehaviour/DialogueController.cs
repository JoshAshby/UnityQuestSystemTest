using GrandCentral.Builders;

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

            builder.AddEntry("One Robin")
                .AddCriteron("bird", "robin")
                .AddCriteron("cylinders-seen", 0);

            builder.AddEntry("Two Robins")
                .AddCriteron("bird", "robin")
                .AddCriteron("cylinders-seen", 1);

            builder.AddEntry("Three Robins")
                .AddCriteron("bird", "robin")
                .AddCriteron("cylinders-seen", 2);

            builder.AddEntry("Lots of Robins")
                .AddCriteron("bird", "robin")
                .AddCriteron("cylinders-seen", 3, 10);

            Rules.Add("test", builder.Finalize());
        }
    }
}