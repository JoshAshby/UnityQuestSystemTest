using System.Collections.Generic;
using GrandCentral.Facts;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Facts Controller", true)]
    public class FactsController : Singleton<FactsController>
    {
        public FactDatabase FactDatabase { get; private set; }

        [SerializeField]
        private bool Debug = false;

        private void Awake()
        {
            FactDatabase = new FactDatabase();
            FactDatabase.Add("global", new FactDictionary());
            FactDatabase.Add("player", new FactDictionary());
        }

        private void OnGUI()
        {
            if(!Debug)
                return;

            int c = 1;
            int height = 22;

            foreach (var shard in FactDatabase)
            {
                GUI.Label(new Rect(10, c * height, 300, height), shard.Key);
                c++;

                List<string> toRemove = new List<string>();

                foreach (var entry in shard.Value)
                {
                    string k = string.Format("{0} -> {1}", entry.Key, entry.Value.ToString());
                    if (GUI.Button(new Rect(5, (c * height) + 1, 18, height - 2), "X"))
                        toRemove.Add(entry.Key);

                    GUI.Label(new Rect(30, c * height, 300, height), k);
                    c++;
                }

                foreach (var removal in toRemove)
                {
                    shard.Value.Remove(removal);
                }
            }
        }
    }
}