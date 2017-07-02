using System;
using System.Linq;
using GrandCentral.Criterion;

namespace GrandCentral.Builders
{
    public class DialogueDatabaseBuilder : IDialogueDatabaseBuilder,
                                           IDialogueDatabaseEntryBuilder,
                                           IDialogueDatabaseEntryChoiceStep1Builder,
                                           IDialogueDatabaseEntryChoiceStep2Builder,
                                           IDialogueDatabaseEntryChoiceBuilder
    {
        private DialogueDatabase _database;
        private IDialogueEntryMeta _entry;
        private IDialogueChoice _choice;

        public DialogueDatabaseBuilder()
        {
            _database = new DialogueDatabase();
            _entry = null;
            _choice = null;
        }

        public IDialogueDatabaseEntryBuilder NewEntry(string name)
        {
            _entry = new DialogueEntryMeta(name);
            _choice = null;

            return this;
        }

        IDialogueDatabaseEntryChoiceBuilder IDialogueDatabaseEntryBuilder.DisplayText(string text)
        {
            ((DialogueEntryMeta)_entry).DisplayText = text;
            return this;
        }

        public IDialogueDatabaseEntryChoiceStep1Builder NewChoice(string key)
        {
            _choice = new DialogueChoice(key);
            return this;
        }

        public IDialogueDatabaseEntryChoiceStep1Builder AddCriteron<T>(string key, Func<T, bool> proc)
        {
            _choice.Criterion.Add(new ProcCriterion<T>(key, proc));
            return this;
        }

        public IDialogueDatabaseEntryChoiceStep1Builder AddCriteron<T>(string fact, string key, Func<T, bool> proc)
        {
            _choice.Criterion.Add(new ProcCriterion<T>(fact, key, proc));
            return this;
        }

        IDialogueDatabaseEntryChoiceStep2Builder IDialogueDatabaseEntryChoiceStep1Builder.DisplayText(string name, string text)
        {
            ((DialogueChoice)_choice).Label = name;
            ((DialogueChoice)_choice).DisplayText = text;
            return this;
        }

        public IDialogueDatabaseEntryChoiceBuilder SaveChoice()
        {
            _entry.Choices.Add(_choice);
            _choice = null;
            return this;
        }

        public IDialogueDatabaseBuilder SaveEntry()
        {
            _database.Add(_entry.Key, _entry);
            _entry = null;
            return this;
        }

        public DialogueDatabase Build()
        {
            return _database;
        }
    }
}