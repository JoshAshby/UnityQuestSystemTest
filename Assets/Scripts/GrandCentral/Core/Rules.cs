using System.Collections.Generic;

namespace GrandCentral
{
    public class Rules : Dictionary<string, RulesShard>
    {
        public Query From(string key)
        {
            return Query.From(this[key]);
        }
    }
}