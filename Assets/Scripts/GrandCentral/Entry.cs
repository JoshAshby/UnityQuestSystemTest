using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrandCentral;
using GrandCentral.Criterion;
using GrandCentral.Mutations;

namespace GrandCentral
{
    public class Entry : IEntry
    {
        public string Name { get; internal set; }
        public List<ICriterion> Criteria { get; internal set; }

        public List<IStateMutation> StateMutations { get; internal set; }
        public string NextEntry { get; internal set; }

        public string Payload { get; internal set; }

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

        public bool Check(FactDictionary context, FactDatabase FactDatabase)
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
                    if (FactDatabase[FactKey].ContainsKey(AccessKey))
                        val = FactDatabase[FactKey][AccessKey];
                }

                bool checker = criterion.Check(val);

                string passFai = checker ? "<color=green>Passed</color>" : "<color=red>FAILED</color>";
                string valString = "null";

                if (val != null)
                    valString = val.ToString();

                log += string.Format("\t[{0} {1}] {2}\n", criterion.ToString(), valString, passFai);

                return checker;
            });

            string passFail = check ? "<color=green>Passed</color>" : "<color=red>FAILED</color>";

            string contextLog = "";
            foreach (var con in context)
            {
                contextLog += string.Format("{0} = {1}\n", con.Key, con.Value.ToString());
            }

            Debug.LogFormat("Entry - {0} --resolves--> {1}\n<b>{3}</b>\n--- Context -----------\n{5}----------------------\n{4}--next-->{2}", Name, Payload, NextEntry, passFail, log, contextLog);

            return check;
        }
    }
}