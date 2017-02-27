using GrandCentral.FileCabinet;
using UnityEngine;

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

        private void OnGUI()
        {
            int c = 1;
            int height = 22;

            foreach (var shard in Facts)
            {
                GUI.Label(new Rect(10, c*height, 300, height), shard.Key);
                c++;
                foreach(var entry in shard.Value)
                {
                    string k = string.Format("{0} -> {1}", entry.Key, entry.Value.ToString());
                    GUI.Label(new Rect(20, c*height, 300, height), k);
                    c++;
                }
            }
        }
    }
}