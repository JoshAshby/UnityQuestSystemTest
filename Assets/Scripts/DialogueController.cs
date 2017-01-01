using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace Ashogue
{
    // Used during runtime
    public class DialogueText
    {
        public string Text;
        public List<string> Choices;
    }

    public class DialogueEvents
    {
        public delegate void OnStartHandler();
        public static event OnStartHandler OnStart;

        public delegate void OnNodeHandler(DialogueText text);
        public static event OnNodeHandler OnNode;

        public delegate void OnEndHandler();
        public static event OnEndHandler OnEnd;
    }

    public delegate void DialogueCallback();

    public class DialogueController
    {
        private static string currentDialogue = null;
        private static string currentNode = null;

        private static DialogueCallback currentCallback = null;

        public static string DatabaseLocation = "Ashogue/dialogues.xml";

        private static DialogueContainer dialogues = null;

        public static DialogueEvents events;

        public static void Initialize()
        {
            dialogues = DialogueContainer.Load(Path.Combine(Application.dataPath, DatabaseLocation));
        }

        public static void StartDialogue(string ID)
        { }

        public static void StartDialogue(string ID, DialogueCallback callback)
        { }

        public static void ContinueDialogue()
        { }

        public static void ContinueDialogue(string choice)
        { }

        public static void EndDialogue()
        { }
    }
}