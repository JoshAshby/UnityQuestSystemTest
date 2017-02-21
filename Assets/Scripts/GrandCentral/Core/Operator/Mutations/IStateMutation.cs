namespace GrandCentral
{
    namespace Operator
    {
        namespace Mutations
        {
            interface IStateMutation
            {
                string Fact { get; }
                string AccessKey { get; }

                void Mutate();
            }
        }
    }
}