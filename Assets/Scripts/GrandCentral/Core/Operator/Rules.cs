using System.Collections.Generic;

namespace GrandCentral
{
    namespace Operator
    {
        public class Rules : Dictionary<string, RulesShard>
        {
            public IQuery From(string key, string segment)
            {
                return Query.From(this[key], segment);
            }
        }
    }
}