using System;
using Ashogue.Data;

namespace Ashogue
{
    namespace Extensions
    {
        static class MetadataWrapper
        {
            public static void AddMetadata<TMetadata>(this ANode node) where TMetadata : IMetadata, new()
            {
                string ID = String.Format("Untitled Metadata {0}", node.Metadata.Count);
                TMetadata metadata = new TMetadata { ID = ID };

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void AddMetadata<TMetadata>(this ANode node, string ID) where TMetadata : IMetadata, new()
            {
                TMetadata metadata = new TMetadata { ID = ID };

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void AddMetadata(this ANode node, Type TMetadata)
            {
                string ID = String.Format("Untitled Metadata {0}", node.Metadata.Count);
                IMetadata metadata = Activator.CreateInstance(TMetadata) as IMetadata;
                metadata.ID = ID;

                node.Metadata.Add(ID, metadata);
            }

            public static void AddMetadata(this ANode node, Type TMetadata, string ID)
            {
                IMetadata metadata = Activator.CreateInstance(TMetadata) as IMetadata;
                metadata.ID = ID;

                node.Metadata.Add(ID, metadata);
            }

            public static void AddMetadata(this ANode node, IMetadata metadata)
            {

                node.Metadata.Add(metadata.ID, metadata);
            }

            public static void RenameMetadata(this ANode node, string fromID, string toID)
            {
                IMetadata metadata = node.Metadata[fromID];

                node.Metadata.Remove(fromID);
                metadata.ID = toID;
                node.Metadata.Add(toID, metadata);
            }

            public static void RemoveMetadata(this ANode node, string ID)
            {
                node.Metadata.Remove(ID);
            }
        }

        static class BranchWrapper
        {
            public static void AddBranch<TBranch>(this ABranchedNode node) where TBranch : IBranch, new()
            {
                string ID = String.Format("Untitled Branch {0}", node.Branches.Count);
                TBranch branch = new TBranch { ID = ID };

                node.Branches.Add(branch.ID, branch);
            }

            public static void AddBranch<TBranch>(this ABranchedNode node, string ID) where TBranch : IBranch, new()
            {
                TBranch Branch = new TBranch { ID = ID };

                node.Branches.Add(Branch.ID, Branch);
            }

            public static void AddBranch(this ABranchedNode node, Type TBranch)
            {
                string ID = String.Format("Untitled Branch {0}", node.Branches.Count);
                IBranch Branch = Activator.CreateInstance(TBranch) as IBranch;
                Branch.ID = ID;

                node.Branches.Add(ID, Branch);
            }

            public static void AddBranch(this ABranchedNode node, Type TBranch, string ID)
            {
                IBranch Branch = Activator.CreateInstance(TBranch) as IBranch;

                Branch.ID = ID;

                node.Branches.Add(ID, Branch);
            }

            public static void AddBranch(this ABranchedNode node, IBranch Branch)
            {
                node.Branches.Add(Branch.ID, Branch);
            }

            public static void RenameBranch(this ABranchedNode node, string fromID, string toID)
            {
                IBranch Branch = node.Branches[fromID];

                node.Branches.Remove(fromID);
                Branch.ID = toID;
                node.Branches.Add(toID, Branch);
            }

            public static void RemoveBranch(this ABranchedNode node, string ID)
            {
                node.Branches.Remove(ID);
            }
        }

        static class NodeWrapper
        {
            public static void AddNode<TNode>(this Dialogue dialogue) where TNode : ANode, new()
            {
                string ID = String.Format("Untitled Node {0}", dialogue.Nodes.Count);
                TNode node = new TNode { ID = ID };

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode<TNode>(this Dialogue dialogue, string ID) where TNode : ANode, new()
            {
                TNode node = new TNode { ID = ID };

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, Type TNode)
            {
                string ID = String.Format("Untitled Node {0}", dialogue.Nodes.Count);
                ANode node = Activator.CreateInstance(TNode) as ANode;

                node.ID = ID;

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, Type TNode, string ID)
            {
                ANode node = Activator.CreateInstance(TNode) as ANode;

                node.ID = ID;

                dialogue.Nodes.Add(ID, node);
            }

            public static void AddNode(this Dialogue dialogue, ANode node)
            {
                dialogue.Nodes.Add(node.ID, node);
            }

            public static void RenameNode(this Dialogue dialogue, string fromID, string toID)
            {
                ANode node = dialogue.Nodes[fromID];

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