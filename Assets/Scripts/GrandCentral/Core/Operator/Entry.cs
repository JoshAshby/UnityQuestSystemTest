using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrandCentral.Operator.Criterion;
using GrandCentral.Operator.Mutations;

namespace GrandCentral
{
    namespace Operator
    {
        internal class Entry : IEntry
        {
            public string Name { get; internal set; }
            public List<ICriterion> Criteria { get; internal set; }
            public List<IStateMutation> StateMutations { get; internal set; }

            public string Payload { get; internal set; }
            public string NextEntry { get; internal set; }

            public Entry(string segment)
            {
                Name = segment;
                Criteria = new List<ICriterion>();
                StateMutations = new List<IStateMutation>();
            }

            public int Length
            {
                get { return Criteria.Count; }
            }

            public bool Check(StateShard context)
            {
                string log = "";

                bool check = Criteria.All(criterion =>
                {
                    object val = null;

                    string FactKey = criterion.FactKey;
                    string AccessKey = criterion.AccessKey;

                    if (context.ContainsKey(FactKey))
                        FactKey = (string)context[FactKey];

                    if (context.ContainsKey(AccessKey))
                        val = context[AccessKey];
                    else
                    {
                        if (StateController.Instance.State[FactKey].ContainsKey(AccessKey))
                            val = StateController.Instance.State[FactKey][AccessKey];
                    }

                    bool checker = criterion.Check(val);

                    string passFai = checker ? "<color=green>Passed</color>" : "<color=red>FAILED</color>";
                    string valString = "null";

                    if (val != null)
                        valString = val.ToString();

                    log += string.Format("[{0} {1} {2}]", criterion.ToString(), passFai, valString);

                    return checker;
                });

                string passFail = check ? "<color=green>Passed</color>" : "<color=red>FAILED</color>";

                Debug.LogFormat("Entry {0} -> {1} => {2}\n{3}: {4}", Name, Payload, NextEntry, passFail, log);

                return check;
            }
        }
    }
}