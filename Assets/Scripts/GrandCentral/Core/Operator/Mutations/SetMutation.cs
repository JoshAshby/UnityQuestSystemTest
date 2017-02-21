namespace GrandCentral
{
    namespace Operator
    {
        namespace Mutations
        {
            class SetMutation<T> : IStateMutation
            {
                public string Fact { get; internal set; }
                public string AccessKey { get; internal set; }

                protected T _setVal;

                public SetMutation(string fact, string key, T val)
                {
                    Fact = fact;
                    AccessKey = key;

                    _setVal = val;
                }

                public void Mutate()
                {
                    State state = StateController.Instance.State;

                    if (!state.ContainsKey(Fact))
                        state.Add(Fact, new StateShard());

                    if (!state[Fact].ContainsKey(AccessKey))
                        state[Fact].Add(AccessKey, default(T));

                    state[Fact][AccessKey] = _setVal;
                }
            }
        }
    }
}