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
            public string Segment { get; internal set; }
            public List<ICriterion> Criteria { get; internal set; }
            public List<IStateMutation> StateMutations { get; internal set; }

            public string Payload { get; internal set; }
            public string NextEntry { get; internal set; }

            public Entry(string segment)
            {
                Segment = segment;
                Criteria = new List<ICriterion>();
                StateMutations = new List<IStateMutation>();
            }

            public int Length
            {
                get { return Criteria.Count; }
            }

            public bool Check(IQuery query)
            {
                string log = "";

                bool check = Criteria.All(criterion =>
                {
                    object val = null;

                    if (criterion.FactKey == "query")
                    {
                        query.Context.TryGetValue(criterion.AccessKey, out val);
                    }
                    else
                    {
                        StateController.Instance.State[criterion.FactKey].TryGetValue(criterion.AccessKey, out val);
                    }

                    bool checker = criterion.Check(val);

                    string passFai = checker ? "Passed" : "FAILED";
                    string valString;

                    if (val == null)
                        valString = "null";
                    else
                        valString = val.ToString();

                    log += string.Format("[({0}) {1} {2}]", criterion.ToString(), passFai, valString);

                    return checker;
                });

                string passFail = check ? "Passed" : "FAILED";

                Debug.LogFormat("Entry {0} {1}: {2}", Segment, passFail, log);

                return check;
            }
        }
    }
}