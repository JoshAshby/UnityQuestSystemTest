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

            public string Query(IQuery query)
            {
                IEntry entry = Entries.Where(x => x.Segment == query.Segment).FirstOrDefault(x => x.Check(query));

                if (entry == null)
                    return null;

                entry.StateMutations.ForEach(x => x.Mutate());

                return entry.Payload;
            }
        }
    }
}