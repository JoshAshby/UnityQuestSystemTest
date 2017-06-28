namespace GrandCentral
{
    public interface IEvent { }

    public interface IHandle<T> where T : IEvent
    {
        void Handle(T message);
    }
}