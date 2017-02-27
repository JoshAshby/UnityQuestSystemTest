using GrandCentral.FileCabinet;
using GrandCentral.Switchboard;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Dialogue Controller", true)]
    public class DialogueController : Singleton<DialogueController>
    {
        public void RequestLine(string character, string line, FactShard context)
        {
            Debug.LogFormat("DialogueController - Got request for {1} from {0}", character, line);
            IEntry entry = SwitchboardController.Instance.QueryFor(character, line, context);

            if (entry != null)
                TelegraphController.Instance.Bus.Publish<DialogueRequest>(new DialogueRequest { Entry = entry });
        }
    }
}