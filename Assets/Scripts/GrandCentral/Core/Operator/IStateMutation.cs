namespace GrandCentral
{
    namespace Operator
    {
        interface IStateMutation
        {
            string Fact { get; }
            string AccessKey { get; }

            void Mutate();
        }

        class SetMutation<T> : IStateMutation
        {
            public string Fact { get; internal set; }
            public string AccessKey { get; internal set; }

            protected T _setVal;

            public SetMutation(string fact, string key, T val)
            {
                Fact = fact;
                AccessKey = key;

                _setVal  = val;
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

        class DecrementMutation : IStateMutation
        {
            public string Fact { get; internal set; }
            public string AccessKey { get; internal set; }

            protected int _byVal;

            public DecrementMutation(string fact, string key, int val)
            {
                Fact = fact;
                AccessKey = key;

                _byVal  = val;
            }

            public void Mutate()
            {
                State state = StateController.Instance.State;

                if (!state.ContainsKey(Fact))
                    state.Add(Fact, new StateShard());
                
                if (!state[Fact].ContainsKey(AccessKey))
                    state[Fact].Add(AccessKey, default(int));

                state[Fact][AccessKey] = (int)state[Fact][AccessKey] - _byVal;
            }
        }

        class IncrementMutation : IStateMutation
        {
            public string Fact { get; internal set; }
            public string AccessKey { get; internal set; }

            protected int _byVal;

            public IncrementMutation(string fact, string key, int val)
            {
                Fact = fact;
                AccessKey = key;

                _byVal  = val;
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