using System;
using System.IO;
using UnityEngine;
using Ashogue.Data;
using System.Linq;
using System.Collections.Generic;

namespace Ashogue
{
    public class StartedEventArgs : EventArgs
    {
        public string DialogueID { get; internal set; }
    }

    public class NodeEventArgs : EventArgs
    {
        public string DialogueID { get; internal set; }
        public string NodeID { get; internal set; }

        public Dictionary<string, IMetadata> Metadata { get; internal set; }
    }

    public class DialogueEventArgs : EventArgs
    {
        public string DialogueID { get; internal set; }
        public string NodeID { get; internal set; }

        public string Text { get; internal set; }
        public List<string> Branches { get; internal set; }

        public Dictionary<string, IMetadata> Metadata { get; internal set; }
    }

    public class MessageEventArgs : EventArgs
    {
        public string DialogueID { get; internal set; }
        public string NodeID { get; internal set; }

        public string Message { get; internal set; }

        public Dictionary<string, IMetadata> Metadata { get; internal set; }
    }

    public class EndedEventArgs : EventArgs
    {
        public string DialogueID { get; internal set; }
        public string NodeID { get; internal set; }

        public bool Suddenly { get; internal set; }

        public Dictionary<string, IMetadata> Metadata { get; internal set; }
    }

    public static class DialogueController
    {
        public static class Events
        {
            public static event EventHandler<StartedEventArgs> Started;
            internal static void OnStarted(IDialogue dialogue)
            {
                var n = Started;
                if (n != null)
                    n(null, new StartedEventArgs
                    {
                        DialogueID = dialogue.ID
                    });
            }

            public static event EventHandler<NodeEventArgs> Node;
            internal static void OnNode(IDialogue dialogue, INode node)
            {
                var n = Node;
                if (n != null)
                    n(null, new NodeEventArgs
                    {
                        DialogueID = dialogue.ID,
                        NodeID = node.ID,
                        Metadata = node.Metadata
                    });
            }

            public static event EventHandler<DialogueEventArgs> Dialogue;
            internal static void OnDialogue(IDialogue dialogue, TextNode node)
            {
                var n = Dialogue;
                if (n != null)
                    n(null, new DialogueEventArgs
                    {
                        DialogueID = dialogue.ID,
                        NodeID = node.ID,
                        Text = node.Text,
                        Branches = node.Branches.Keys.OrderBy(x => x).ToList(),
                        Metadata = node.Metadata
                    });
            }

            public static event EventHandler<MessageEventArgs> Message;
            internal static void OnMessage(IDialogue dialogue, EventNode node)
            {
                var n = Message;
                if (n != null)
                    n(null, new MessageEventArgs
                    {
                        DialogueID = dialogue.ID,
                        NodeID = node.ID,
                        Message = node.Message,
                        Metadata = node.Metadata
                    });
            }

            public static event EventHandler<EndedEventArgs> Ended;
            internal static void OnEnded(IDialogue dialogue, INode node, bool suddenly = false)
            {
                var n = Ended;
                if (n != null)
                    n(null, new EndedEventArgs
                    {
                        DialogueID = dialogue.ID,
                        NodeID = node.ID,
                        Suddenly = suddenly,
                        Metadata = node.Metadata
                    });
            }
        }

        public static string DatabaseLocation = "Ashogue/dialogues.xml";

        public static DialogueContainer dialogues = null;

        private static IDialogue currentDialogue;
        private static INode currentNode;

        private static Action currentCallback = null;

        public static void Initialize()
        {
            dialogues = DialogueContainer.Load(Path.Combine(Application.dataPath, DatabaseLocation));
        }

        public static void StartDialogue(string ID)
        {
            currentDialogue = dialogues.Dialogues[ID];
            currentNode = currentDialogue.Nodes[currentDialogue.FirstNodeID];

            Events.OnStarted(currentDialogue);
            Progress();
        }

        public static void StartDialogue(string ID, Action callback)
        {
            currentCallback = callback;
            StartDialogue(ID);
        }

        public static void ContinueDialogue()
        {
            string nId = ((IBranchedNode)currentNode).Branches.First().Value.NextNodeID;
            currentNode = currentDialogue.Nodes[nId];

            Progress();
        }

        public static void ContinueDialogue(string choice)
        {
            string nId = ((IBranchedNode)currentNode).Branches[choice].NextNodeID;
            currentNode = currentDialogue.Nodes[nId];

            Progress();
        }

        public static void EndDialogue(INode node, bool suddenly = false)
        {
            currentDialogue = null;
            currentNode = null;

            currentCallback.Invoke();
            Events.OnEnded(currentDialogue, node, suddenly);
            currentCallback = null;
        }

        private static void Progress()
        {
            INode interNode = currentNode;

            while (!(interNode is IBranchedNode))
            {
                Events.OnNode(currentDialogue, interNode);

                if (interNode is EventNode)
                    HandleEventNode((EventNode)interNode);

                else if (interNode is WaitNode)
                    HandleWaitNode((WaitNode)interNode);

                else if ((interNode is EndNode) || !(interNode is IBranchedNode))
                {
                    bool suddenly = !(interNode is EndNode) && (interNode is IBranchedNode);

                    EndDialogue(interNode, suddenly);
                    return;
                }

                interNode = currentDialogue.Nodes[((IBranchedNode)interNode).Branches.First().Value.NextNodeID];
            }

            TextNode node = (TextNode)interNode;

            Events.OnNode(currentDialogue, interNode);
            currentNode = interNode;
        }

        private static void HandleEventNode(EventNode node)
        {
            Events.OnMessage(currentDialogue, node);
        }

        private static void HandleWaitNode(WaitNode node)
        {

        }
    }
}