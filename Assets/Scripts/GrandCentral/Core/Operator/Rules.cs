using System.Collections.Generic;

namespace GrandCentral
{
    namespace Operator
    {
        public class Rules : Dictionary<string, RulesShard>
        {
            public string QueryFor(string partition, string name, StateShard context)
            {
                return this[partition].QueryFor(name, context);
            }
        }
    }
}