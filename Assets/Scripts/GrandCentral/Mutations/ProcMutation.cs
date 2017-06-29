using System;

namespace GrandCentral.Mutations
{
    public class ProcMutation<T> : IStateMutation
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
            FactDatabase state = Pannier.FactDatabase;

            if (!state.ContainsKey(Fact))
                state.Add(Fact, new FactDictionary());

            if (!state[Fact].ContainsKey(AccessKey))
                state[Fact].Add(AccessKey, default(T));

            state[Fact][AccessKey] = _setter((T)state[Fact][AccessKey]);
        }
    }
}