namespace GrandCentral
{
    internal interface ICriteron
    {
        string AccessKey { get; }

        bool Check(Query query);
    }
}