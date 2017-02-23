using GrandCentral.FileCabinet;

namespace GrandCentral
{
    [Prefab("Facts Controller", true)]
    public class FactsController : Singleton<FactsController>
    {
        public Facts Facts { get; private set; }

        private void Awake()
        {
            Facts = new Facts();
            Facts.Add("global", new FactShard());
            Facts.Add("player", new FactShard());
        }
    }
}