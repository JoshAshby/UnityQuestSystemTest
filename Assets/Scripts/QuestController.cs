using System;
using UnityEngine;
using Zenject;

namespace Ashest
{
    public class QuestController : MonoBehaviour
    {
        [SerializeField]
        private QuestDatabase Quests;

        private void Start()
        {
            Debug.LogFormat("Loading {0} quests", Quests.Quests.Count);

            foreach (Quest quest in Quests.Quests)
            {
                // Yolo
                Debug.AssertFormat(!String.IsNullOrEmpty(quest.ID), "Quest {0} has an empty ID", quest.DisplayName);
                Debug.AssertFormat(!String.IsNullOrEmpty(quest.DisplayName), "Quest {0} has an empty DisplayName", quest.ID);
            }
        }
    }
}