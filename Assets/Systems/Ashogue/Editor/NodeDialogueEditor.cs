using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace Ashogue
{
    namespace Editor
    {
        class DialogueCanvas : NodeCanvas
        {
            public DialogueCanvas() : base()
            {
                State.AddNode(typeof(StartNodeCanvasNode), new Vector2(10, 10));
            }
        }

        public class NodeDialogueEditor : EditorWindow
        {
            private static NodeDialogueEditor _editor;
            public static NodeDialogueEditor editor { get { AssureEditor(); return _editor; } }
            public static void AssureEditor() { if (_editor == null) OpenEditor(); }

            private DialogueContainer database;
            private string path;

            private string currentDialogue = "test";

            private DialogueCanvas Canvas;

            [MenuItem("Window/Ashogue/Dialogue Editor")]
            public static NodeDialogueEditor OpenEditor()
            {
                _editor = EditorWindow.GetWindow<NodeDialogueEditor>();
                _editor.titleContent = new GUIContent("Dialogue Editor");

                return _editor;
            }

            private void EnsureDatabase()
            {
                if (database != null)
                    return;

                path = Path.Combine(Application.dataPath, DialogueController.DatabaseLocation);

                if (File.Exists(path))
                    LoadDatabase();
                else
                    CreateDatabase();
            }

            private void CreateDatabase()
            {
                Debug.Log("Dialogue database doesn't exist, creating");
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                database = new DialogueContainer();
                database.Save(path);
            }

            private void LoadDatabase()
            {
                database = DialogueContainer.Load(path);
            }

            private void Setup()
            {
                EnsureDatabase();
                SelectDialogue(currentDialogue);
            }

            private void SelectDialogue(string id)
            {
                if (Canvas != null)
                {
                    Canvas.EventSystem.AddNode -= AddNodeHandler;
                    Canvas.EventSystem.RemoveNode -= RemoveNodeHandler;
                    Canvas.EventSystem.AddConnection -= AddConnectionHandler;
                    Canvas.EventSystem.RemoveConnection -= RemoveConnectionHandler;

                    Canvas.Repaint -= Repaint;
                }

                Canvas = new DialogueCanvas();
                Canvas.Repaint += Repaint;

                IDialogue dialogue = database.Dialogues[id];

                Ashode.INode startNode = Canvas.State.Nodes.First();

                IDialogueNode firstNode = null;
                foreach (var dialogueNode in dialogue.Nodes.Values)
                {
                    Vector2 pos = dialogueNode.Position;
                    IDialogueNode canvasNode = null;

                    Type dialogueNodeType = dialogueNode.GetType();

                    if (typeof(TextNode).IsAssignableFrom(dialogueNodeType))
                        canvasNode = Canvas.State.AddNode<TextNodeCanvasNode>(pos);

                    else if (typeof(EventNode).IsAssignableFrom(dialogueNodeType))
                        canvasNode = Canvas.State.AddNode<EventNodeCanvasNode>(pos);

                    else if (typeof(WaitNode).IsAssignableFrom(dialogueNodeType))
                        canvasNode = Canvas.State.AddNode<WaitNodeCanvasNode>(pos);

                    else if (typeof(EndNode).IsAssignableFrom(dialogueNodeType))
                        canvasNode = Canvas.State.AddNode<EndNodeCanvasNode>(pos);

                    canvasNode.Target = dialogueNode;

                    if (dialogueNode.ID == dialogue.FirstNodeID)
                        firstNode = canvasNode;

                    if (!typeof(IBranchedNode).IsAssignableFrom(dialogueNode.GetType()))
                        continue;

                    IBranchedNode branchedNode = (IBranchedNode)dialogueNode;
                    foreach (var branch in branchedNode.Branches.Values)
                    {
                        canvasNode.AddKnob(branch.ID, NodeSide.Right, 1, Direction.Output, typeof(string));
                    }
                }

                Canvas.State.AddConnection(startNode.Knobs["start"], firstNode.Knobs["in"]);

                foreach (var _node in dialogue.Nodes.Values)
                {
                    if (!typeof(IBranchedNode).IsAssignableFrom(_node.GetType()))
                        continue;

                    IBranchedNode node = (IBranchedNode)_node;

                    IDialogueNode FromNode = (IDialogueNode)Canvas.State.Nodes.Where(x => typeof(IDialogueNode).IsAssignableFrom(x.GetType())).ToList().Find(x => ((IDialogueNode)x).Target.ID == node.ID);
                    foreach (var branch in node.Branches.Values)
                    {
                        IKnob FromKnob = FromNode.Knobs[branch.ID];
                        IDialogueNode ToNode = (IDialogueNode)Canvas.State.Nodes.Where(x => typeof(IDialogueNode).IsAssignableFrom(x.GetType())).ToList().Find(x => ((IDialogueNode)x).Target.ID == branch.NextNodeID);
                        IKnob ToKnob = ToNode.Knobs["in"];

                        Canvas.State.AddConnection(FromKnob, ToKnob);
                    }
                }

                Canvas.EventSystem.AddNode += AddNodeHandler;
                Canvas.EventSystem.RemoveNode += RemoveNodeHandler;
                Canvas.EventSystem.AddConnection += AddConnectionHandler;
                Canvas.EventSystem.RemoveConnection += RemoveConnectionHandler;

                Canvas.Repaint += Repaint;
            }

            private void AddNodeHandler(object sender, TargetEventArgs<Ashode.INode> e)
            {
                IDialogue dialogue = database.Dialogues[currentDialogue];
                IDialogueNode node = (IDialogueNode)e.Target;
                Type nodeType = e.Target.GetType();

                if (typeof(TextNodeCanvasNode).IsAssignableFrom(nodeType))
                    node.Target = dialogue.AddNode<TextNode>();

                else if (typeof(EventNodeCanvasNode).IsAssignableFrom(nodeType))
                    node.Target = dialogue.AddNode<EventNode>();

                else if (typeof(WaitNodeCanvasNode).IsAssignableFrom(nodeType))
                    node.Target = dialogue.AddNode<WaitNode>();

                else if (typeof(EndNodeCanvasNode).IsAssignableFrom(nodeType))
                    node.Target = dialogue.AddNode<EndNode>();
            }

            private void RemoveNodeHandler(object sender, TargetEventArgs<Ashode.INode> e)
            {
                IDialogue dialogue = database.Dialogues[currentDialogue];
                IDialogueNode node = (IDialogueNode)e.Target;
                Type nodeType = e.Target.GetType();

                dialogue.RemoveNode(node.Target.ID);
            }

            private void AddConnectionHandler(object sender, TargetEventArgs<Ashode.IConnection> e)
            {
                IConnection conn = e.Target;
                IDialogue dialogue = database.Dialogues[currentDialogue];
                IDialogueNode FromNode = (IDialogueNode)e.Target.FromNode;
                IDialogueNode ToNode = (IDialogueNode)e.Target.ToNode;

                ((IBranchedNode)dialogue.Nodes[FromNode.Target.ID]).AddBranch<SimpleBranch>(conn.ID).NextNodeID = ToNode.Target.ID;
            }

            private void RemoveConnectionHandler(object sender, TargetEventArgs<Ashode.IConnection> e)
            {
                IConnection conn = e.Target;
                IDialogue dialogue = database.Dialogues[currentDialogue];
                IDialogueNode FromNode = (IDialogueNode)e.Target.FromNode;
                IDialogueNode ToNode = (IDialogueNode)e.Target.ToNode;

                IBranch branch = FromNode.OfTargetType<IBranchedNode>().Target.Branches.First(x => x.Value.NextNodeID == ToNode.ID).Value;

                ((IBranchedNode)dialogue.Nodes[FromNode.Target.ID]).RemoveBranch(branch.ID);
            }

            private void OnDestroy()
            {
                Canvas.EventSystem.AddNode -= AddNodeHandler;
                Canvas.EventSystem.RemoveNode -= RemoveNodeHandler;

                Canvas.EventSystem.AddConnection -= AddConnectionHandler;
                Canvas.EventSystem.RemoveConnection -= RemoveConnectionHandler;

                Canvas.Repaint -= Repaint;
            }

            float toolbarHeight = 0;
            int sideWindowWidth = 300;
            private void OnGUI()
            {
                if (Canvas == null)
                    Setup();

                GUILayout.BeginHorizontal(EditorStyles.toolbar);
                if (GUILayout.Button("Add Dialogue", EditorStyles.toolbarButton))
                    database.AddDialogue();

                if (GUILayout.Button("Save Database", EditorStyles.toolbarButton))
                    database.Save(path);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reload Database", EditorStyles.toolbarButton))
                    ReloadDatabase();
                GUILayout.EndHorizontal();

                if(Event.current.type == EventType.Repaint)
                    toolbarHeight = GUILayoutUtility.GetLastRect().yMax;

                Rect sideWindowRect = new Rect(0, toolbarHeight, sideWindowWidth, position.height);

                GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
                if (GUILayout.Button("Save"))
                    SaveState();

                foreach(var dialogue in database.Dialogues.Values)
                {
                    if(GUILayout.Button(dialogue.ID))
                        SelectDialogue(dialogue.ID);
                }
                GUILayout.EndArea();

                Rect canvasRect = new Rect(sideWindowWidth, toolbarHeight, position.width - sideWindowWidth, position.height);
                Canvas.Draw(canvasRect);
            }

            private void SaveState()
            {
                database.Save(path);
            }

            private void ReloadDatabase()
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reload the database? This will lose any unsaved work", "Yup", "NO!"))
                {
                    LoadDatabase();
                }
            }
        }
    }
}