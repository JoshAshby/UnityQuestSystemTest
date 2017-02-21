
namespace GrandCentral
{
    namespace Operator
    {
        namespace Mutations
        {
            class IncrementMutation : IStateMutation
            {
                public string Fact { get; internal set; }
                public string AccessKey { get; internal set; }

                protected int _byVal;

                public IncrementMutation(string fact, string key, int val)
                {
                    Fact = fact;
                    AccessKey = key;

                    _byVal = val;
                }

                public void Mutate()
                {
                    State state = StateController.Instance.State;

                    if (!state.ContainsKey(Fact))
                        state.Add(Fact, new StateShard());

                    if (!state[Fact].ContainsKey(AccessKey))
                        state[Fact].Add(AccessKey, default(int));

                    state[Fact][AccessKey] = (int)state[Fact][AccessKey] + _byVal;
                }
            }
        }
    }
}