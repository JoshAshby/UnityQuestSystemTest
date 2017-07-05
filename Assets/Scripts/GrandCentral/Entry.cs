using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GrandCentral;
using GrandCentral.Criterion;
using GrandCentral.Mutations;
using System.Xml.Serialization;

namespace GrandCentral
{
    public class Entry : IEntry
    {
        public string EventName { get; internal set; }

        public string Name { get; internal set; }

        [XmlIgnore]
        public List<ICriterion> Criteria { get; internal set; }

        [XmlArray("Criteria")]
        public ICriterion[] XmlCriteria
        {
            get { return Criteria.ToArray(); }
            set { Criteria = value.ToList(); }
        }

        [XmlIgnore]
        public List<IMutation> BlackboardMutations { get; internal set; }

        [XmlArray("BlackboardMutations")]
        public IMutation[] XmlBlackboardMutations
        {
            get { return BlackboardMutations.ToArray(); }
            set { BlackboardMutations = value.ToList(); }
        }

        public Entry(string segment)
        {
            Name = segment;
            Criteria = new List<ICriterion>();
            BlackboardMutations = new List<IMutation>();
        }

        public Entry() { }

        public int Length
        {
            get { return Criteria.Count; }
        }

        public bool Check(Blackboard context, BlackboardsContainer FactDatabase)
        {
            string log = "";

            bool check = Criteria.All(criterion =>
            {
                object val = null;

                string BlackboardHint = criterion.Hint;
                string FactKey = criterion.FactKey;

                if (context.ContainsKey(FactKey))
                    val = context.Get<object>(FactKey);
                else
                {
                    if (context.ContainsKey(BlackboardHint))
                        BlackboardHint = context.Get<string>(BlackboardHint);

                    val = FactDatabase.Get<object>(BlackboardHint, FactKey);
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
            foreach (var con in context.XmlData)
            {
                contextLog += string.Format("{0} = {1}\n", con.Key, con.Value.ToString());
            }

            Debug.LogFormat("Entry - {0}\n<b>{1}</b>\n--- Context -----------\n{3}----------------------\n{2}", Name, passFail, log, contextLog);

            return check;
        }
    }
}