using GrandCentral.FileCabinet;
using GrandCentral.Switchboard;
using GrandCentral.Telegraph;

namespace GrandCentral
{
    public class DialogueRequest : IMessage {
        public IEntry Entry { get; set; }
    }
    
    [Prefab("Dialogue Controller")]
    public class DialogueController : Singleton<DialogueController>
    {
        public void Request(string character, string name, FactShard context)
        {
            IEntry entry = SwitchboardController.Instance.QueryFor(character, name, context);

            if (entry != null)
                TelegraphController.Instance.Bus.Publish<DialogueRequest>(new DialogueRequest{ Entry = entry });
        }
    }
}