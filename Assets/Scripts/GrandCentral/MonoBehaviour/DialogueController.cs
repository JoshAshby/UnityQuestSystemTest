using System.Collections.Generic;
using GrandCentral.FileCabinet;
using GrandCentral.Switchboard;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Dialogue Controller", true)]
    public class DialogueController : Singleton<DialogueController>
    {
        public static void RequestLine(string character, string line, FactShard context)
        {
            Debug.LogFormat("DialogueController - Got request for {1} from {0}", character, line);
            IEntry entry = SwitchboardController.QueryFor(character, line, context);

            if (entry != null)
                TelegraphController.Publish<DialogueRequest>(new DialogueRequest { Entry = entry });
        }

        protected Dictionary<string, string> Lines;

        public void Awake()
        {
            Lines = new Dictionary<string, string>();
        }
    }
}