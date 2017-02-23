using System;

namespace GrandCentral.Operator.Mutations
{
    class ProcMutation<T> : IStateMutation
    {
        public string Fact { get; internal set; }
        public string AccessKey { get; internal set; }

        protected Func<T, T> _setter;

        public ProcMutation(string fact, string key, Func<T, T> setter)
        {
            Fact = fact;
            AccessKey = key;

            _setter = setter;
        }

        public void Mutate()
        {
            Facts state = FactsController.Instance.Facts;

            if (!state.ContainsKey(Fact))
                state.Add(Fact, new FactShard());

            if (!state[Fact].ContainsKey(AccessKey))
                state[Fact].Add(AccessKey, default(T));

            state[Fact][AccessKey] = _setter((T)state[Fact][AccessKey]);
        }
    }
}