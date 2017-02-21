using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral
{
    namespace Operator
    {
        internal interface IEntry
        {
            string Segment { get; }
            List<ICriterion> Criteria { get; }
            List<IStateMutation> StateMutations { get; }

            string Payload { get; }
            string NextEntry { get; }

            int Length { get; }
            bool Check(IQuery query);
        }

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