﻿using UnityEngine;
using GrandCentral.FileCabinet;

namespace GrandCentral
{
    public class DialogueTest : MonoBehaviour, IInteractiveBehaviour
    {
        [SerializeField]
        private string BirdType;

        public void OnLookEnter() { }
        public void OnLookStay() { }
        public void OnLookExit() { }

        public void OnInteract()
        {
            FactShard context = new FactShard();
            context.Add("bird", BirdType);

            string res = SwitchboardController.Instance.QueryFor("protag", "seen-robin", context);

            Debug.Log(res);
        }
    }
}