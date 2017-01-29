using System;
using Ashogue.Data;

namespace Ashogue
{
    namespace Extensions
    {
        static class MetadataWrapper
        {
            public static IMetadata AddMetadata<TMetadata>(this INode node, string ID = null) where TMetadata : IMetadata, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IMetadata metadata = new TMetadata { ID = ID };
                node.Metadata.Add(ID, metadata);

                return metadata;
            }

            public static IMetadata AddMetadata(this INode node, Type TMetadata, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IMetadata metadata = (IMetadata)Activator.CreateInstance(TMetadata);

                metadata.ID = ID;
                node.Metadata.Add(ID, metadata);

                return metadata;
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

        static class BranchWrapper
        {
            public static IBranch AddBranch<TBranch>(this IBranchedNode node, string ID = null) where TBranch : IBranch, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IBranch branch = new TBranch { ID = ID };
                node.Branches.Add(ID, branch);

                return branch;
            }

            public static IBranch AddBranch(this IBranchedNode node, Type TBranch, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IBranch Branch = (IBranch)Activator.CreateInstance(TBranch);

                Branch.ID = ID;
                node.Branches.Add(ID, Branch);

                return Branch;
            }

            public static void RenameBranch(this IBranchedNode node, string fromID, string toID)
            {
                IBranch Branch = node.Branches[fromID];

                node.Branches.Remove(fromID);
                Branch.ID = toID;
                node.Branches.Add(toID, Branch);
            }

            public static void RemoveBranch(this IBranchedNode node, string ID)
            {
                node.Branches.Remove(ID);
            }
        }

        static class NodeWrapper
        {
            public static INode AddNode<TNode>(this IDialogue dialogue, string ID = null) where TNode : INode, new()
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                INode node = new TNode { ID = ID };
                dialogue.Nodes.Add(ID, node);

                return node;
            }

            public static INode AddNode(this IDialogue dialogue, Type TNode, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                INode node = (INode)Activator.CreateInstance(TNode);

                node.ID = ID;
                dialogue.Nodes.Add(ID, node);

                return node;
            }

            public static void RenameNode(this IDialogue dialogue, string fromID, string toID)
            {
                INode node = dialogue.Nodes[fromID];

                dialogue.Nodes.Remove(fromID);
                node.ID = toID;
                dialogue.Nodes.Add(toID, node);
            }

            public static void RemoveNode(this IDialogue dialogue, string ID)
            {
                dialogue.Nodes.Remove(ID);
            }
        }

        static class DialogueContainerWrapper
        {
            public static IDialogue AddDialogue(this DialogueContainer container, string ID = null)
            {
                if (String.IsNullOrEmpty(ID))
                    ID = Guid.NewGuid().ToString();

                IDialogue dialogue = new Dialogue { ID = ID };
                container.Dialogues.Add(ID, dialogue);

                return dialogue;
            }

            public static void RenameDialogue(this DialogueContainer container, string fromID, string toID)
            {
                IDialogue dialogue = container.Dialogues[fromID];

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