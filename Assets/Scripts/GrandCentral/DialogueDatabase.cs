using System.Collections.Generic;
using System.Linq;
using GrandCentral.Criterion;

namespace GrandCentral
{
    public interface IDialogueChoice
    {
        string Key { get; }

        string Label { get; }
        string DisplayText { get; }

        List<ICriterion> Criterion { get; }
    }

    public interface IDialogueEntry
    {
        string Key { get; }

        string DisplayText { get; }

        List<IDialogueChoice> Choices { get; }
    }

    public interface IDialogueEntryMeta
    {
        string Key { get; }

        string DisplayText { get; }

        List<IDialogueChoice> Choices { get; }

        IDialogueEntry Evaluate(FactDatabase FactDatabase, FactDictionary context);
    }

    public class DialogueChoice : IDialogueChoice
    {
        public string Key { get; internal set; }

        public string Label { get; internal set; }
        public string DisplayText { get; internal set; }

        public List<ICriterion> Criterion { get; internal set; }

        public DialogueChoice(string key)
        {
            Key = key;
            Criterion = new List<ICriterion> ();
        }
    }

    public class DialogueEntry : IDialogueEntry
    {
        public string Key { get; internal set; }

        public string DisplayText { get; internal set; }

        public List<IDialogueChoice> Choices { get; internal set; }
    }

    public class DialogueEntryMeta : IDialogueEntryMeta
    {
        public string Key { get; internal set; }

        public string DisplayText { get; internal set; }

        public List<IDialogueChoice> Choices { get; internal set; }

        public DialogueEntryMeta(string key)
        {
            Key = key;
            Choices = new List<IDialogueChoice> ();
        }

        public IDialogueEntry Evaluate(FactDatabase FactDatabase, FactDictionary context)
        {
            List<IDialogueChoice> choices = Choices.FindAll(choice => {
                return choice.Criterion.All(criterion => {
                    object val = null;

                    string FactKey = criterion.FactKey;
                    string AccessKey = criterion.AccessKey;

                    if (context.ContainsKey(AccessKey))
                        val = context[AccessKey];
                    else
                    {
                        if (context.ContainsKey(FactKey))
                            FactKey = (string)context[FactKey];

                        if (FactDatabase[FactKey].ContainsKey(AccessKey))
                            val = FactDatabase[FactKey][AccessKey];
                    }

                    return criterion.Check(val);
                });
            });

            return new DialogueEntry {
                Key = Key,
                DisplayText = DisplayText,
                Choices = choices
            };
        }
    }

    public class DialogueDatabase : Dictionary<string, IDialogueEntryMeta> { }
}