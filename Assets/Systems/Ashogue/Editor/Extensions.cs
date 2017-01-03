using System;

namespace Ashogue {
    namespace Extensions {
        static class MetadataWrapper
        {
            public static void AddMetadata<TMetadata>(this INode node) where TMetadata : IMetadata, new()
            {
                string ID = String.Format("Untitled Metadata {0}", node.Metadata.Count);
                TMetadata metadata = new TMetadata { ID = ID };

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void AddMetadata<TMetadata>(this INode node, string ID) where TMetadata : IMetadata, new()
            {
                TMetadata metadata = new TMetadata { ID = ID };

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void AddMetadata(this INode node, IMetadata metadata)
            {

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void RenameMetadata(this INode node, string fromID, string toID)
            {
                IMetadata metadata = node.Metadata[fromID];

                node.Metadata.Remove(fromID);
                metadata.ID = toID;
                node.Metadata.Add(toID, metadata);
            }

            public static void RemoveMetadata(this INode node, string ID)
            {
                node.Metadata.Remove(ID);
            }
        }

        static class ChoiceWrapper
        {
            public static void AddChoice<TChoice>(this IChoiceNode node) where TChoice : IChoice, new()
            {
                string ID = String.Format("Untitled Choice {0}", node.Choices.Count);
                TChoice choice = new TChoice { ID = ID };

                node.Choices.Add(choice.ID, choice);
            }

            public static void AddChoice<TChoice>(this IChoiceNode node, string ID) where TChoice : IChoice, new()
            {
                TChoice choice = new TChoice { ID = ID };

                node.Choices.Add(choice.ID, choice);
            }

            public static void Addchoice(this IChoiceNode node, IChoice choice)
            {
                node.Choices.Add(choice.ID, choice);
            }

            public static void RenameChoice(this IChoiceNode node, string fromID, string toID)
            {
                IChoice choice = node.Choices[fromID];

                node.Choices.Remove(fromID);
                choice.ID = toID;
                node.Choices.Add(toID, choice);
            }

            public static void RemoveChoice(this IChoiceNode node, string ID)
            {
                node.Choices.Remove(ID);
            }
        }

        static class NodeWrapper
        {
            public static void AddNode<TNode>(this Dialogue dialogue) where TNode : INode, new()
            {
                string ID = String.Format("Untitled Node {0}", dialogue.Nodes.Count);
                TNode node = new TNode { ID = ID };

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode<TNode>(this Dialogue dialogue, string ID) where TNode : INode, new()
            {
                TNode node = new TNode { ID = ID };

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, Type TNode)
            {
                string ID = String.Format("Untitled Node {0}", dialogue.Nodes.Count);
                INode node = (INode)Activator.CreateInstance(TNode);

                node.ID = ID;

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, Type TNode, string ID)
            {
                INode node = (INode)Activator.CreateInstance(TNode);

                node.ID = ID;

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, INode node)
            {
                dialogue.Nodes.Add(node.ID, node);
            }

            public static void RenameNode(this Dialogue dialogue, string fromID, string toID)
            {
                INode node = dialogue.Nodes[fromID];

                dialogue.Nodes.Remove(fromID);
                node.ID = toID;
                dialogue.Nodes.Add(toID, node);
            }

            public static void RemoveNode(this Dialogue dialogue, string ID)
            {
                dialogue.Nodes.Remove(ID);
            }
        }

        static class DialogueContainerWrapper
        {
            public static void AddDialogue(this DialogueContainer container)
            {
                string ID = String.Format("Untitled Dialogue {0}", container.Dialogues.Count);
                container.Dialogues.Add(ID, new Dialogue { ID = ID });
            }

            public static void AddDialogue(this DialogueContainer container, string ID)
            {
                container.Dialogues.Add(ID, new Dialogue { ID = ID });
            }

            public static void RenameDialogue(this DialogueContainer container, string fromID, string toID)
            {
                Dialogue dialogue = container.Dialogues[fromID];

                container.Dialogues.Remove(fromID);
                dialogue.ID = toID;
                container.Dialogues.Add(toID, dialogue);
            }

            public static void RemoveDialogue(this DialogueContainer container, string ID)
            {
                container.Dialogues.Remove(ID);
            }
        }
    }
}