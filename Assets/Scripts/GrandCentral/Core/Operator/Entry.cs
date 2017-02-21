using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

        internal interface IEntry
        {
            string Segment { get; }
            List<ICriteron> Criteria { get; }
            List<IStateMutation> StateMutations { get; }

            string Payload { get; }
            string NextEntry { get; }

            int Length { get; }
            bool Check(IQuery query);
        }

        internal class Entry : IEntry
        {
            public string Segment { get; internal set; }
            public List<ICriteron> Criteria { get; internal set; }
            public List<IStateMutation> StateMutations { get; internal set; }

            public string Payload { get; internal set; }
            public string NextEntry { get; internal set; }

            public Entry(string segment)
            {
                Segment = segment;
                Criteria = new List<ICriteron>();
                StateMutations = new List<IStateMutation>();
            }

            public int Length
            {
                get { return Criteria.Count; }
            }

            public bool Check(IQuery query)
            {
                string log = "";

                bool check = Criteria.All(criterion => {
                    object val = null;

                    if (criterion.FactKey == "query")
                    {
                        val = query.Context[criterion.AccessKey];
                    } else {
                        val = StateController.Instance.State[criterion.FactKey][criterion.AccessKey];
                    }

                    bool checker = criterion.Check(val);

                    string passFai = checker ? "Passed" : "FAILED";
                    log += string.Format("[({0}) {1} {2}]", criterion.ToString(), passFai, val.ToString());

                    return checker;
                });

                string passFail = check ? "Passed" : "FAILED";

                Debug.LogFormat("Entry {0} {1}: {2}", Segment, passFail, log);

                return check;
            }
        }
    }
}