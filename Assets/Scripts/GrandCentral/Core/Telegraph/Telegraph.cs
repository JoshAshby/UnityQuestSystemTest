namespace GrandCentral.Telegraph
{
    public interface IMessage { }

    public interface IHandle<T> where T : IMessage
    {
        void Handle(T message);
    }
}