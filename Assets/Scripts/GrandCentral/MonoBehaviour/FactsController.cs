namespace GrandCentral
{
    [Prefab("Facts Controller", true)]
    public class FactsController : Singleton<FactsController>
    {
        public Facts Facts { get; }

        private void Awake()
        {
            Facts.Add("global", new FactShard());
            Facts.Add("player", new FactShard());
        }
    }
}