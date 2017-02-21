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
                IEntry entry = Entries.FirstOrDefault(x => x.Check(query) == true);

                if (entry == null)
                    return null;

                return entry.Payload;
            }
        }
    }
}