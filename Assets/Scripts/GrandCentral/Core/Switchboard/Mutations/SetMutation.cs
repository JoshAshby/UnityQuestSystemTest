using GrandCentral.FileCabinet;

namespace GrandCentral.Switchboard.Mutations
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
            Facts state = FactsController.Instance.Facts;

            if (!state.ContainsKey(Fact))
                state.Add(Fact, new FactShard());

            if (!state[Fact].ContainsKey(AccessKey))
                state[Fact].Add(AccessKey, default(T));

            state[Fact][AccessKey] = _setVal;
        }
    }
}