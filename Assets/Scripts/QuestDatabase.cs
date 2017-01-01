using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ashest
{
    public interface IQuestCondition
    {
        bool CheckRequirements();
    }

    [Serializable]
    public enum QuestState
    {
        NotStarted,
        Progressing,
        Success,
        Failure
    }

    [Serializable]
    public class QuestStage
    {
        public string ID;
        public string DisplayName;

        public string Description;

        public QuestState State;

        // Randomly select one if we've got more than one
        public List<string> StartDialog; // For now this can be a displayed string and later a key to the dialog system?
        public List<AudioClip> StartVoiceOver; // Perhaps with the dialog system this should be obsolete?
        // public List<string> StartAudioNames; // Made this a string for now, assuming that the audio manager would in a name of a track

        public List<String> SuccessDialog;
        public List<AudioClip> SuccessVoiceOver;
        // public List<string> SuccessAudioNames;

        // public List<string> FailureDialog;
        // public List<AudioClip> FailureVoiceOver;
        // public List<string> FailureAudioNames;

        // public List<IQuestCondition> StartConditions;

        // public List<IQuestCondition> CompleteConditions;
    }

    [Serializable]
    public class Quest
    {
        public string ID;
        public string DisplayName;

        public string Description;

        public QuestState State;

        public QuestStage EntryStage;
        public List<QuestStage> Stages;
    }

    [CreateAssetMenu(fileName = "QuestDatabase.asset", menuName = "New Quest Database", order = 0)]
    public class QuestDatabase : ScriptableObject
    {
        public string ID;
        public string DisplayName;

        public List<Quest> Quests;
    }
}