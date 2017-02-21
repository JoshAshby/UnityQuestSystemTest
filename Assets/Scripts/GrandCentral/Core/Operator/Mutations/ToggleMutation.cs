namespace GrandCentral
{
    namespace Operator
    {
        namespace Mutations
        {
            class ToggleMutation : IStateMutation
            {
                public string Fact { get; internal set; }
                public string AccessKey { get; internal set; }

                public ToggleMutation(string fact, string key)
                {
                    Fact = fact;
                    AccessKey = key;
                }

                public void Mutate()
                {
                    State state = StateController.Instance.State;

                    if (!state.ContainsKey(Fact))
                        state.Add(Fact, new StateShard());

                    if (!state[Fact].ContainsKey(AccessKey))
                        state[Fact].Add(AccessKey, default(bool));

                    state[Fact][AccessKey] = !(bool)state[Fact][AccessKey];
                }
            }
        }
    }
}