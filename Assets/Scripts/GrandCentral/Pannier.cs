using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using GrandCentral.Builders;

namespace GrandCentral
{
    public static class Pannier
    {
        private static bool built = false;

        private static FactDatabase _factDatabase = null;
        private static RuleDatabase _ruleDatabase = null;
        private static DialogueDatabase _dialogueDatabase = null;

        public static FactDatabase FactDatabase
        {
            get { initDB(); return _factDatabase; }
            private set { _factDatabase = value; }
        }

        public static RuleDatabase RuleDatabase
        {
            get { initDB(); return _ruleDatabase; }
            private set { _ruleDatabase = value; }
        }

        public static DialogueDatabase DialogueDatabase
        {
           get { initDB(); return _dialogueDatabase; }
           private set { _dialogueDatabase = value; }
        }

        public static IEntry Request(string line, FactDictionary context)
        {
            IEntry entry = RuleDatabase.QueryFor(line, context, FactDatabase);

            if (entry != null)
                EventBus.Publish<ContextAwareEvent>(new ContextAwareEvent { Entry = entry });

            return entry;
        }

        public static IDialogueEntry GetDialogue(string line, FactDictionary context)
        {
            return DialogueDatabase[ line ].Evaluate( FactDatabase, context );
        }

        public static IDialogueEntry RequestDialogue(string line, FactDictionary context)
        {
            IEntry entry = RuleDatabase.QueryFor(line, context, FactDatabase);
            IDialogueEntry dialogueEntry = GetDialogue( entry.Payload, context );

            if (entry != null)
                EventBus.Publish<DialogueEvent>(new DialogueEvent { DialogueEntry = dialogueEntry });

            return dialogueEntry;
        }

        private static void initDB()
        {
            if(built)
                return;

            FactDatabase = new FactDatabase
            {
                { "global", new FactDictionary() },
                { "player", new FactDictionary() }
            };

            IRuleDatabaseBuilder ruleBuilder = new RuleDatabaseBuilder();
            IDialogueDatabaseBuilder dialogueBuilder = new DialogueDatabaseBuilder();

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 0)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("seen-one-robin");

            dialogueBuilder
                .NewEntry("seen-one-robin")
                    .DisplayText("You've seen one robin!")
                    .NewChoice("seen-one-robin-01")
                        .AddCriteron<string>("bird", x => x == "robin")
                        .DisplayText("Chase it", "Should I chase it?")
                        .SaveChoice()
                    .NewChoice("seen-one-robin-02")
                        .DisplayText("Leave it", "I should leave it alone ...")
                        .SaveChoice()
                    .SaveEntry();

            ruleBuilder
                .AddEntry("seen-one-robin-01")
                    .ReturnPayload("seen-one-robin-01");

            dialogueBuilder
                .NewEntry("seen-one-robin-01")
                    .DisplayText("Yes, you should chase it!")
                    .SaveEntry();

            ruleBuilder
                .AddEntry("seen-one-robin-02")
                    .ReturnPayload("seen-one-robin-02");

            dialogueBuilder
                .NewEntry("seen-one-robin-02")
                    .DisplayText("I'll leave it alone")
                    .SaveEntry();

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 1)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("two-robins");

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 2)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("three-robins");

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("speaker", x => x == "protag")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x == 3)
                    .AddCriteron<bool>("global", "seen-many-robins-01", x => !x)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .SetFact<bool>("global", "seen-many-robins-01", true)
                    .ReturnPayload("many-robins-03");

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("speaker", x => x == "protag")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("many-robins-01");

            ruleBuilder
                .AddEntry("seen-robin")
                    .AddCriteron<string>("bird", x => x == "robin")
                    .AddCriteron<int>("global", "cylinders-seen", x => x >= 3)
                    .OnMatch()
                    .MutateFact<int>("global", "cylinders-seen", x => x + 1)
                    .ReturnPayload("many-robins-02");

            ruleBuilder
                .AddEntry("seen-one-robin-03")
                    .ReturnPayload("seen-three-robins");

            ruleBuilder
                .AddEntry("seen-many-robin-01")
                    .ReturnPayload("seen-many-robins");

            ruleBuilder
                .AddEntry("seen-many-robin-02")
                    .ReturnPayload("seen-many-robin-twice");

            ruleBuilder
                .AddEntry("seen-many-robin-03")
                    .ReturnPayload("seen-many-robin-three");

            RuleDatabase = ruleBuilder.Build();
            DialogueDatabase = dialogueBuilder.Build();
        }
    }
}