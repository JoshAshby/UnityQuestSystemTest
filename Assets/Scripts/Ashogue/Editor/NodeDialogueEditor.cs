using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using System;
using System.Linq;
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

            private string currentDialogueID = "";
            private IDialogue CurrentDialogue { get { return database.Dialogues[currentDialogueID]; } }

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

            private void ReloadDatabase()
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reload the database? This will lose any unsaved work", "Yup", "NO!"))
                {
                    LoadDatabase();
                    SelectDialogue(database.Dialogues.Keys.First());
                }
            }

            private void Setup()
            {
                EnsureDatabase();
                currentDialogueID = database.Dialogues.Keys.First();

                SelectDialogue(currentDialogueID);
            }

            private void SelectDialogue(string id)
            {
                currentDialogueID = id;

                if (Canvas != null)
                {
                    Canvas.EventSystem.AddNode -= AddNodeHandler;
                    Canvas.EventSystem.RemoveNode -= RemoveNodeHandler;

                    Canvas.EventSystem.AddConnection -= AddConnectionHandler;
                    Canvas.EventSystem.RemoveConnection -= RemoveConnectionHandler;

                    Canvas.Repaint -= Repaint;
                }

                Canvas = new DialogueCanvas();

                Ashode.INode startNode = Canvas.State.Nodes.First();

                IDialogueNode firstNode = null;
                foreach (var dialogueNode in CurrentDialogue.Nodes.Values)
                {
                    Vector2 pos = dialogueNode.Position;
                    IDialogueNode canvasNode = null;

                    Type dialogueNodeType = dialogueNode.GetType();

                    if (dialogueNode is TextNode)
                        canvasNode = Canvas.State.AddNode<TextNodeCanvasNode>(pos);

                    else if (dialogueNode is EventNode)
                        canvasNode = Canvas.State.AddNode<EventNodeCanvasNode>(pos);

                    else if (dialogueNode is WaitNode)
                        canvasNode = Canvas.State.AddNode<WaitNodeCanvasNode>(pos);

                    else if (dialogueNode is EndNode)
                        canvasNode = Canvas.State.AddNode<EndNodeCanvasNode>(pos);

                    canvasNode.Target = dialogueNode;

                    if (dialogueNode.ID == CurrentDialogue.FirstNodeID)
                        firstNode = canvasNode;

                    if (!(dialogueNode is IBranchedNode))
                        continue;

                    IBranchedNode branchedNode = (IBranchedNode)dialogueNode;
                    foreach (var branch in branchedNode.Branches.Values)
                        canvasNode.AddKnob(branch.ID, NodeSide.Right, 1, Direction.Output, typeof(string));
                }

                if (firstNode != null)
                    Canvas.State.AddConnection(startNode.Knobs["start"], firstNode.Knobs["in"]);

                foreach (var _node in CurrentDialogue.Nodes.Values)
                {
                    if (_node is IBranchedNode)
                    {
                        IBranchedNode node = (IBranchedNode)_node;

                        IDialogueNode FromNode = (IDialogueNode)Canvas.State.Nodes.Where(x => x is IDialogueNode).ToList().Find(x => ((IDialogueNode)x).Target.ID == node.ID);
                        foreach (var branch in node.Branches.Values)
                        {
                            if (String.IsNullOrEmpty(branch.NextNodeID))
                                continue;

                            IKnob FromKnob = FromNode.Knobs[branch.ID];
                            IDialogueNode ToNode = (IDialogueNode)Canvas.State.Nodes.Where(x => x is IDialogueNode).ToList().Find(x => ((IDialogueNode)x).Target.ID == branch.NextNodeID);
                            IKnob ToKnob = ToNode.Knobs["in"];

                            Canvas.State.AddConnection(FromKnob, ToKnob);
                        }
                    }
                    else if (_node is INextedNode)
                    {
                        INextedNode node = (INextedNode)_node;

                        IDialogueNode FromNode = (IDialogueNode)Canvas.State.Nodes.Where(x => x is IDialogueNode).ToList().Find(x => ((IDialogueNode)x).Target.ID == node.ID);
                        IDialogueNode ToNode = (IDialogueNode)Canvas.State.Nodes.Where(x => x is IDialogueNode).ToList().Find(x => ((IDialogueNode)x).Target.ID == node.NextNodeID);

                        IKnob FromKnob = FromNode.Knobs["out"];
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
                IDialogueNode node = (IDialogueNode)e.Target;
                Type nodeType = e.Target.GetType();

                if (node is TextNodeCanvasNode)
                    node.Target = CurrentDialogue.AddNode<TextNode>();

                else if (node is EventNodeCanvasNode)
                    node.Target = CurrentDialogue.AddNode<EventNode>();

                else if (node is WaitNodeCanvasNode)
                    node.Target = CurrentDialogue.AddNode<WaitNode>();

                else if (node is EndNodeCanvasNode)
                    node.Target = CurrentDialogue.AddNode<EndNode>();
            }

            private void RemoveNodeHandler(object sender, TargetEventArgs<Ashode.INode> e)
            {
                IDialogueNode node = (IDialogueNode)e.Target;

                CurrentDialogue.RemoveNode(node.Target.ID);
            }

            private void AddConnectionHandler(object sender, TargetEventArgs<Ashode.IConnection> e)
            {
                IConnection conn = e.Target;

                IDialogueNode ToNode = (IDialogueNode)conn.ToNode;

                if (conn.FromNode is StartNodeCanvasNode)
                {
                    CurrentDialogue.FirstNodeID = ToNode.Target.ID;
                }
                else if (conn.FromNode is TextNodeCanvasNode)
                {
                    IDialogueNode FromNode = (IDialogueNode)conn.FromNode;

                    IBranchedNode dialogueNode = (IBranchedNode)CurrentDialogue.Nodes[FromNode.Target.ID];

                    dialogueNode.Branches[conn.FromKnob.ID].NextNodeID = ToNode.Target.ID;
                }
                else if (conn.FromNode is EventNodeCanvasNode || conn.FromNode is WaitNodeCanvasNode)
                {
                    IDialogueNode FromNode = (IDialogueNode)conn.FromNode;

                    INextedNode dialogueNode = (INextedNode)CurrentDialogue.Nodes[FromNode.Target.ID];

                    dialogueNode.NextNodeID = ToNode.Target.ID;
                }
            }

            private void RemoveConnectionHandler(object sender, TargetEventArgs<Ashode.IConnection> e)
            {
                IConnection conn = e.Target;

                if (conn.FromNode is StartNodeCanvasNode)
                {
                    CurrentDialogue.FirstNodeID = "";
                    return;
                }

                IDialogueNode FromNode = (IDialogueNode)conn.FromNode;

                if (FromNode.TargetType is IBranchedNode)
                    ((IBranchedNode)CurrentDialogue.Nodes[FromNode.Target.ID]).Branches[conn.FromKnob.ID].NextNodeID = "";

                if (FromNode.TargetType is INextedNode)
                    ((INextedNode)CurrentDialogue.Nodes[FromNode.Target.ID]).NextNodeID = "";
            }

            private void OnDestroy()
            {
                if (Canvas == null)
                    return;

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
                    SaveState();

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Reload Database", EditorStyles.toolbarButton))
                    ReloadDatabase();
                GUILayout.EndHorizontal();

                if (Event.current.type == EventType.Repaint)
                    toolbarHeight = GUILayoutUtility.GetLastRect().yMax;

                Rect sideWindowRect = new Rect(0, toolbarHeight, sideWindowWidth, position.height);
                GUILayout.BeginArea(sideWindowRect, GUI.skin.box);

                foreach (var dialogue in database.Dialogues.Values)
                    if (GUILayout.Button(dialogue.Name))
                        SelectDialogue(dialogue.ID);

                if (!String.IsNullOrEmpty(currentDialogueID))
                {
                    GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name", GUILayout.ExpandWidth(false));
                    CurrentDialogue.Name = GUILayout.TextField(CurrentDialogue.Name, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();
                }

                GUILayout.EndArea();

                Rect canvasRect = new Rect(sideWindowWidth, toolbarHeight, position.width - sideWindowWidth, position.height);
                Canvas.Draw(canvasRect);
            }

            private void SaveState()
            {
                database.Save(path);
            }
        }
    }
}