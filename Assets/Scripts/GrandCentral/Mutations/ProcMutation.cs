using System;

namespace GrandCentral.Mutations
{
    public class ProcMutation<T> : IMutation
    {
        public string Hint { get; internal set; }
        public string FactKey { get; internal set; }

        protected Func<T, T> _setter;

        public ProcMutation(string fact, string key, Func<T, T> setter)
        {
            Hint = fact;
            FactKey = key;

            _setter = setter;
        }

        public void Mutate(BlackboardsContainer state)
        {
            state.Set<T>(Hint, FactKey, _setter(state.Get<T>(Hint, FactKey)));
        }
    }
}