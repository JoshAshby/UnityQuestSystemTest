namespace GrandCentral.Mutations
{
    public interface IStateMutation
    {
        string Fact { get; }
        string AccessKey { get; }

        void Mutate(FactDatabase database);
    }
}