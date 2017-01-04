using System;
using System.IO;
using UnityEngine;
using Ashogue.Data;

namespace Ashogue
{
    // Used during runtime
    public class DialogueText
    {
        public string Text;
        public string[] Branches;
    }

    public class NodeEventArgs : EventArgs { public DialogueText text { get; set; } }
    public class MessageEventArgs : EventArgs { public string message { get; set; } }

    public static class DialogueController
    {
        public static string DatabaseLocation = "Ashogue/dialogues.xml";

        private static DialogueContainer dialogues = null;

        private static Dialogue currentDialogue;
        private static ANode currentNode;

        private static Action currentCallback = null;

        public static class Events
        {
            public static event EventHandler Started;
            internal static void OnStarted()
            {
                var n = Started;
                if (n != null)
                    n(null, null);
            }

            public static event EventHandler<NodeEventArgs> Node;
            internal static void OnNode(DialogueText text)
            {
                var n = Node;
                if (n != null)
                    n(null, new NodeEventArgs { text = text });
            }

            public static event EventHandler Ended;
            internal static void OnEnded()
            {
                var n = Ended;
                if (n != null)
                    n(null, null);
            }

            public static event EventHandler<MessageEventArgs> Message;
            internal static void OnMessage(string message)
            {
                var n = Message;
                if (n != null)
                    n(null, new MessageEventArgs { message = message });
            }
        }

        public static void Initialize()
        {
            dialogues = XmlContainer<DialogueContainer>.Load(Path.Combine(Application.dataPath, DatabaseLocation));
        }

        public static void StartDialogue(string ID)
        {
            currentDialogue = dialogues.Dialogues[ID];
            currentNode = currentDialogue.Nodes[currentDialogue.StartNode];

            Progress();
        }

        public static void StartDialogue(string ID, Action callback)
        {
            currentCallback = callback;
            StartDialogue(ID);
        }

        public static void ContinueDialogue()
        { }

        public static void ContinueDialogue(string choice)
        { }

        public static void EndDialogue()
        { }

        private static void Progress()
        {
            while (!(currentNode is IDisplayNode))
            {

            }

            IDisplayNode node = currentNode as IDisplayNode;

            DialogueText text = new DialogueText
            {
                Text = node.DisplayText(),
                Branches = node.DisplayBranches()
            };

            Events.OnNode(text);
        }
    }
}