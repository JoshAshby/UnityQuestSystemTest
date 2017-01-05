using System;
using System.IO;
using UnityEngine;
using Ashogue.Data;
using System.Linq;
using System.Collections.Generic;

namespace Ashogue
{
    // Used during runtime
    public class DialogueText
    {
        public string Text;
        public string[] Branches;
    }

    public class NodeEventArgs : EventArgs { public DialogueText text { get; set; } }
    public class EndedEventArgs : EventArgs { public bool suddenly { get; set; } }
    public class MessageEventArgs : EventArgs { public string message { get; set; } }

    public static class DialogueController
    {
        public static string DatabaseLocation = "Ashogue/dialogues.xml";

        public static DialogueContainer dialogues = null;

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

            public static event EventHandler<EndedEventArgs> Ended;
            internal static void OnEnded(bool suddenly = false)
            {
                var n = Ended;
                if (n != null)
                    n(null, new EndedEventArgs { suddenly = suddenly });
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
            currentNode = currentDialogue.Nodes[currentDialogue.FirstNodeID];

            Events.OnStarted();
            Progress();
        }

        public static void StartDialogue(string ID, Action callback)
        {
            currentCallback = callback;
            StartDialogue(ID);
        }

        public static void ContinueDialogue()
        {
            string nId = ((ABranchedNode)currentNode).Branches.First().Value.NextNodeID;
            currentNode = currentDialogue.Nodes[nId];
            Progress();
        }

        public static void ContinueDialogue(string choice)
        {
            string nId = ((ABranchedNode)currentNode).Branches[choice].NextNodeID;
            currentNode = currentDialogue.Nodes[nId];
            Progress();
        }

        public static void EndDialogue()
        {
            currentDialogue = null;
            currentNode = null;

            Events.OnEnded();
        }

        private static void Progress()
        {
            ANode interNode = currentNode;

            while (!(interNode is TextNode))
            {
                if (interNode is EventNode)
                {
                    EventNode n = (EventNode)interNode;
                    Events.OnMessage(n.Message);
                }
                else if (interNode is WaitNode)
                {
                    WaitNode n = (WaitNode)interNode;
                }
                else if (interNode is EndNode)
                {
                    EndDialogue();
                    return;
                }
                interNode = currentDialogue.Nodes[((AChainedNode)interNode).NextNodeID];
            }

            TextNode node = (TextNode)interNode;

            DialogueText text = new DialogueText
            {
                Text = node.Text,
                Branches = node.Branches.Keys.ToArray()
            };

            currentNode = interNode;
            Events.OnNode(text);
        }
    }
}