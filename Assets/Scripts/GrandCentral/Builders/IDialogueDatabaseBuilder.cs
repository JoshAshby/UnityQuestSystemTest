using System;

namespace GrandCentral.Builders
{
    public interface IDialogueDatabaseBuilder
    {
        IDialogueDatabaseEntryBuilder NewEntry(string name);
        DialogueDatabase Build();
    }

    public interface IDialogueDatabaseEntryBuilder
    {
        IDialogueDatabaseEntryChoiceBuilder DisplayText(string text);
        IDialogueDatabaseBuilder SaveEntry();
    }

    public interface IDialogueDatabaseEntryChoiceBuilder
    {
        IDialogueDatabaseEntryChoiceStep1Builder NewChoice(string key);
        IDialogueDatabaseBuilder SaveEntry();
    }

    public interface IDialogueDatabaseEntryChoiceStep1Builder
    {
        IDialogueDatabaseEntryChoiceStep1Builder AddCriteron<T>(string fact, string key, Func<T, bool> proc);
        IDialogueDatabaseEntryChoiceStep1Builder AddCriteron<T>(string key, Func<T, bool> proc);

        IDialogueDatabaseEntryChoiceStep2Builder DisplayText(string name, string text);
    }

    public interface IDialogueDatabaseEntryChoiceStep2Builder
    {
        IDialogueDatabaseEntryChoiceBuilder SaveChoice();
    }
}