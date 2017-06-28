namespace GrandCentral
{
    public class ContextAwareEvent : IEvent {
        public IEntry Entry { get; set; }
    }
}