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
            FactDatabase = new FactDatabase { { "global", new FactDictionary() }, { "player", new FactDictionary() } };

            RuleDatabase = new RuleDBBuilder()
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 0)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("one-robin")
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 1)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("two-robins")
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 2)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("three-robins")
                .AddEntry("seen-robin")
                    .AddCriteron<string>("speaker", x => x == "protag")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 3)
                    .AddCriteron<bool>("global", "seen-many-robins-01", x => !x)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .FactSet<bool>("global", "seen-many-robins-01", true)
                    .ReturnPayload("many-robins-03")
                .AddEntry("seen-robin")
                    .AddCriteron<string>("speaker", x => x == "protag")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("many-robins-01")
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                    .OnMatch()
                    .FactMutate<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("many-robins-02")
                .AddEntry("seen-one-robin-01")
                    .ReturnPayload("seen-one-robin")
                .AddEntry("seen-two-robins-01")
                    .ReturnPayload("seen-two-robins")
                .AddEntry("seen-three-robins-01")
                    .ReturnPayload("seen-three-robins")
                .AddEntry("seen-many-robins-01")
                    .ReturnPayload("seen-many-robins")
                .AddEntry("seen-many-robins-02")
                    .ReturnPayload("seen-many-robins-twice")
                .AddEntry("seen-many-robins-03")
                    .ReturnPayload("seen-many-robins-three")
                .Finalize();
        }
    }
}