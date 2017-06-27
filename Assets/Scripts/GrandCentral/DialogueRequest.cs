using GrandCentral.Events;
using GrandCentral.Switchboard;

public class DialogueRequest : IEvent {
    public IEntry Entry { get; set; }
}