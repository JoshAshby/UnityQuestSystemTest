using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral
{
    namespace Operator
    {
        internal interface IEntry
        {
            string Name { get; }
            List<ICriteron> Criteria { get; }

            string Payload { get; }
            string NextEntry { get; }

            int Length { get; }
            bool Check(IQuery query);
        }

        internal class Entry : IEntry
        {
            public string Name { get; internal set; }
            public List<ICriteron> Criteria { get; internal set; }

            public string Payload { get; internal set; }
            public string NextEntry { get; internal set; }

            public Entry(string name)
            {
                Name = name;
                Criteria = new List<ICriteron>();
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
                        val = query.Context[criterion.AccessKey];
                    else
                        val = StateController.Instance.State[criterion.FactKey][criterion.AccessKey];

                    bool checker = criterion.Check(val);

                    string passFai = checker ? "Passed" : "FAILED";
                    log += string.Format("[({0}) {1} {2}]", criterion.ToString(), passFai, val.ToString());

                    return checker;
                });

                string passFail = check ? "Passed" : "FAILED";

                Debug.LogFormat("Entry {0} {1}: {2}", Name, passFail, log);

                return check;
            }
        }
    }
}