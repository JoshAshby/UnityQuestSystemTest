using System.Collections.Generic;

namespace GrandCentral
{
    namespace Operator
    {
        public class Rules : Dictionary<string, RulesShard>
        {
            public IQuery From(string key)
            {
                return Query.From(this[key]);
            }
        }
    }
}