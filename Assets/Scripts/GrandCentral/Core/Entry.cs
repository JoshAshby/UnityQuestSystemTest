using System.Collections.Generic;
using System.Linq;

namespace GrandCentral
{
    internal class Entry
    {
        internal List<ICriteron> Criteria;
        public object Payload;

        public Entry()
        {
            Criteria = new List<ICriteron>();
        }

        public int Length
        {
            get { return Criteria.Count; }
        }

        public bool Check(Query query)
        {
            return Criteria.All(x => x.Check(query));
        }
    }
}