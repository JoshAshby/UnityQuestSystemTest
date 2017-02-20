using System.Collections.Generic;
using System.Linq;

namespace GrandCentral
{
    public class RulesShard
    {
        internal List<Entry> Entries;

        public RulesShard()
        {
            Entries = new List<Entry>();
        }

        public object Query(Query query)
        {
            Entry entry = Entries.FirstOrDefault(x => x.Check(query));

            if (entry == null)
                return null;

            return entry.Payload;
        }
    }
}