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

            private void ReloadDatabase()
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to reload the database? This will lose any unsaved work", "Yup", "NO!"))
                {
                    LoadDatabase();
                }
            }

            private void Setup()
            {
                EnsureDatabase();

                Canvas = new DialogueCanvas();
                Canvas.Repaint += Repaint;

                Ashode.INode startNode = Canvas.State.Nodes.First();

                IDialogueNode firstNode = null;
                foreach (var node in database.Dialogues[currentDialogue].Nodes.Values)
                {
                    Vector2 pos = new Vector2();
                    IDialogueNode nnode = null;

                    if (typeof(TextNode).IsAssignableFrom(node.GetType()))
                        nnode = Canvas.State.AddNode<TextNodeCanvasNode>(pos);

                    else if (typeof(EventNode).IsAssignableFrom(node.GetType()))
                        nnode = Canvas.State.AddNode<EventNodeCanvasNode>(pos);

                    else if (typeof(WaitNode).IsAssignableFrom(node.GetType()))
                        nnode = Canvas.State.AddNode<WaitNodeCanvasNode>(pos);

                    else if (typeof(EndNode).IsAssignableFrom(node.GetType()))
                        nnode = Canvas.State.AddNode<EndNodeCanvasNode>(pos);

                    nnode.Target = node;

                    if (node.ID == database.Dialogues[currentDialogue].FirstNodeID)
                        firstNode = nnode;
                }

                Canvas.State.AddConnection(startNode.Knobs["start"], firstNode.Knobs["in"]);

                foreach (var _node in Canvas.State.Nodes)
                {
                    if (!typeof(IDialogueNode).IsAssignableFrom(_node.GetType()))
                        continue;

                    IDialogueNode node = (IDialogueNode)_node;


                }

                Canvas.EventSystem.AddNodeEvent += AddNodeHandler;
                Canvas.EventSystem.RemoveNodeEvent += RemoveNodeHandler;
            }

            void AddNodeHandler(object sender, NodeEventArgs<Ashode.INode> e)
            {
                Type nodeType = e.Target.GetType();

                if (typeof(TextNodeCanvasNode).IsAssignableFrom(nodeType))
                {
                    ((TextNodeCanvasNode)e.Target).Target = database.Dialogues[currentDialogue].AddNode<TextNode>();
                }
            }

            void RemoveNodeHandler(object sender, NodeEventArgs<Ashode.INode> e)
            {
                Type nodeType = e.Target.GetType();

                if (typeof(TextNodeCanvasNode).IsAssignableFrom(nodeType))
                {
                    database.Dialogues[currentDialogue].RemoveNode(((TextNodeCanvasNode)e.Target).Target.ID);
                }
            }

            private void OnDestroy()
            {
                Canvas.Repaint -= Repaint;
            }

            private void OnGUI()
            {
                if (Canvas == null)
                    Setup();

                int sideWindowWidth = 300;

                Rect sideWindowRect = new Rect(0, 0, sideWindowWidth, position.height);

                GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
                if (GUILayout.Button("Save"))
                    SaveState();
                GUILayout.EndArea();

                Rect canvasRect = new Rect(sideWindowWidth, 0, position.width - sideWindowWidth, position.height);
                Canvas.Draw(canvasRect);
            }

            private void SaveState() { }
        }
    }
}