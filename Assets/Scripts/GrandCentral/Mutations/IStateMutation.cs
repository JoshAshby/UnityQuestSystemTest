namespace GrandCentral.Mutations
{
    public interface IMutation
    {
        string Hint { get; }
        string FactKey { get; }

        void Mutate(BlackboardsContainer BlackboardsContainer);
    }
}