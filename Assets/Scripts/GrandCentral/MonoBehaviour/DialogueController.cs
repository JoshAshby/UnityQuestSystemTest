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
            builder.AddEntry("wat").AddCriteron("test1", "alpha");
            builder.AddEntry("bat").AddCriteron("test2", 6);
            builder.AddEntry("mat").AddCriteron("test3", 9, 12);
            builder.AddEntry("hat")
                .AddCriteron("test4", "robin")
                .AddCriteron("test5", 19)
                .AddCriteron("test6", 0, 6);

            Rules.Add("test", builder.Finalize());
        }
    }
}