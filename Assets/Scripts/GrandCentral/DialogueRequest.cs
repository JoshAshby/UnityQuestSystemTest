using GrandCentral.Telegraph;
using GrandCentral.Switchboard;

public class DialogueRequest : IMessage {
    public IEntry Entry { get; set; }
}