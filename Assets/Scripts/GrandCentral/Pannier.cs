using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrandCentral.Builders;

namespace GrandCentral
{
    public static class Pannier
    {
        public static FactDatabase FactDatabase { get; private set; }
        public static RuleDatabase RuleDatabase { get; private set; }

        public static IEntry Request(string line, FactDictionary context)
        {
            IEntry entry = RuleDatabase.QueryFor(line, context, FactDatabase);

            if (entry != null)
                EventBus.Publish<ContextAwareEvent>(new ContextAwareEvent { Entry = entry });

            return entry;
        }

        public static void Builders()
        {
            FactDatabase = new FactDatabase();

            FactDatabase.Add("global", new FactDictionary());
            FactDatabase.Add("player", new FactDictionary());


            RuleDBBuilder builder = new RuleDBBuilder();

            builder.New();

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x == 0)
                .SetPayload("one-robin")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .SetNextEntry("seen-one-robin-01");

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x == 1)
                .SetPayload("two-robins")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .SetNextEntry("seen-two-robins-01");

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x == 2)
                .SetPayload("three-robins")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .SetNextEntry("seen-three-robins-01");

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("speaker", x => x == "protag")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x == 3)
                .AddCriteron<bool>("global", "seen-many-robins-01", x => !x)
                .SetPayload("many-robins-03")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .FactSet<bool>("global", "seen-many-robins-01", true)
                .SetNextEntry("seen-many-robins-03");

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("speaker", x => x == "protag")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                .SetPayload("many-robins-01")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .SetNextEntry("seen-many-robins-02");

            builder.AddEntry("seen-robin")
                .AddCriteron<string>("bird", x => x == "robin")
                .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                .SetPayload("many-robins-02")
                .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                .SetNextEntry("seen-many-robins-01");

            builder.AddEntry("seen-one-robin-01")
                .SetPayload("seen-one-robin");

            builder.AddEntry("seen-two-robins-01")
                .SetPayload("seen-two-robins");

            builder.AddEntry("seen-three-robins-01")
                .SetPayload("seen-three-robins");

            builder.AddEntry("seen-many-robins-01")
                .SetPayload("seen-many-robins");

            builder.AddEntry("seen-many-robins-02")
                .SetPayload("seen-many-robins-twice");

            builder.AddEntry("seen-many-robins-03")
                .SetPayload("seen-many-robins-three");

            RuleDatabase = builder.Finalize();
        }
    }
}