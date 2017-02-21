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
                return Criteria.All(criterion => {
                    IGenericValue val = null;

                    if (criterion.FactKey == "query")
                        val = query.Context[criterion.AccessKey];
                    else
                        val = StateController.Instance.State[criterion.FactKey][criterion.AccessKey];

                    Debug.Log(val.ToString());

                    return criterion.Check(val);
                });
            }
        }
    }
}