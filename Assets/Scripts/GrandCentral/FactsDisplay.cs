using System.Collections.Generic;
using UnityEngine;
using GrandCentral;

public class FactsDisplay : MonoBehaviour
{
    private void OnGUI()
    {
        int c = 1;
        int height = 22;

        foreach (var shard in Pannier.Instance.BlackboardsContainer.Blackboards)
        {
            GUI.Label(new Rect(10, c * height, 300, height), shard.Key);
            c++;

            List<string> toRemove = new List<string>();

            foreach (var entry in shard.Value.Facts)
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