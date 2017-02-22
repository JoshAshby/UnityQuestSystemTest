using System;
using System.Collections.Generic;
using System.Linq;

namespace GrandCentral
{
    namespace Operator
    {
        public class RulesShard
        {
            internal List<IEntry> Entries;

            public RulesShard()
            {
                Entries = new List<IEntry>();
            }

            public string QueryFor(string name, StateShard context)
            {
                IEntry entry = Entries.Where(x => x.Name == name).FirstOrDefault(x => x.Check(context));

                if (entry == null)
                    return null;

                entry.StateMutations.ForEach(x => x.Mutate());

                return entry.Payload;
            }
        }
    }
}