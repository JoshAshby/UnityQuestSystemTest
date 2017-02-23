namespace GrandCentral.Operator.Mutations
{
    interface IStateMutation
    {
        string Fact { get; }
        string AccessKey { get; }

        void Mutate();
    }
}