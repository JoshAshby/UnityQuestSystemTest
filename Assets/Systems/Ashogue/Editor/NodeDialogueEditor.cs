using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using Ashogue.Extensions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Ashogue
{
    namespace Editor
    {
        public static class MetadataEditor
        {
            public static List<Type> AllSubTypes<T>()
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)))
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            public static List<Type> AllImplementTypes<T>()
            {
                return AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(T)))
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            public static List<Type> metadataTypes = AllImplementTypes<IMetadata>();

            public static string ChoiceSelector(string[] options, string selected, Action<string> callback)
            {
                int choiceIndex = Array.IndexOf(options, selected);
                if (choiceIndex == -1)
                    choiceIndex = 0;

                EditorGUI.BeginChangeCheck();
                choiceIndex = EditorGUILayout.Popup(choiceIndex, options);
                if (EditorGUI.EndChangeCheck())
                {
                    string newSelection = options[choiceIndex];
                    callback(newSelection);
                    return newSelection;
                }

                return selected;
            }

            private static Dictionary<string, string> nodeTypeChoice = new Dictionary<string, string>();
            public static string TypeField(List<Type> options, string buttonText, Action<string> callback)
            {
                if (!options.Any())
                    return null;

                string[] optionNames = options.Select(x => x.Name).OrderBy(x => x).ToArray();
                if (!nodeTypeChoice.ContainsKey(buttonText))
                    nodeTypeChoice.Add(buttonText, optionNames.First());

                ChoiceSelector(
                    optionNames,
                    nodeTypeChoice[buttonText],
                    (newTypeName) => { nodeTypeChoice[buttonText] = newTypeName; }
                );

                if (GUILayout.Button(buttonText))
                    callback(nodeTypeChoice[buttonText]);

                return nodeTypeChoice[buttonText];
            }

            public static void Editor(Ashogue.Data.INode Target)
            {
                GUILayout.BeginHorizontal();
                TypeField(
                    metadataTypes,
                    "Add Metadata",
                    (newTypeName) => { Target.AddMetadata(metadataTypes.Find(type => type.Name == newTypeName)); }
                );
                GUILayout.EndHorizontal();

                IMetadata removalMetadata = null;

                foreach (IMetadata metadata in Target.Metadata.Values)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Key");
                    metadata.ID = EditorGUILayout.TextField(metadata.ID);

                    if (metadata.Type == typeof(bool))
                    {
                        IMetadata<bool> handle = metadata.OfType<bool>();
                        handle.Value = GUILayout.Toggle(handle.Value, "");
                    }
                    else if (metadata.Type == typeof(float))
                    {
                        IMetadata<float> handle = metadata.OfType<float>();
                        handle.Value = EditorGUILayout.FloatField(handle.Value);
                    }
                    else if (metadata.Type == typeof(string))
                    {
                        IMetadata<string> handle = metadata.OfType<string>();
                        handle.Value = EditorGUILayout.TextField(handle.Value);
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("X"))
                        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this metadata?", "Yup", "NO!"))
                            removalMetadata = metadata;

                    GUILayout.EndHorizontal();
                }

                if (removalMetadata != null)
                    Target.RemoveMetadata(removalMetadata.ID);
            }
        }

        class DialogueCanvas : NodeCanvas
        {
            public DialogueCanvas() : base()
            {
                State.AddNode(typeof(StartNodeCanvasNode), new Vector2(10, 10));
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Hidden = true)]
        class StartNodeCanvasNode : Node
        {
            public override Vector2 MinSize { get { return new Vector2(100, 40); } }
            public override bool CanResize { get { return false; } }

            public override string Title { get { return "Start Node"; } }

            public override bool Removable { get { return false; } }

            public StartNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

            public override void SetupKnobs()
            {
                AddKnob("start", NodeSide.Right, 1, Direction.Output, typeof(string)).Removable = false;
            }

            public override void OnGUI()
            {
                DrawKnob("start", Rect.size.y / 2);
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Name = "Text Node")]
        class TextNodeCanvasNode : Node
        {
            public override Vector2 MinSize { get { return new Vector2(200, 300); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Text Node"; } }

            public TextNode Target { get; }

            public TextNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos)
            {
                Target = new TextNode();
                Target.Text = "";
            }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
            }

            private string _RemoveKnob = null;
            public override void OnGUI()
            {
                DrawKnob("in", 20);

                GUILayout.BeginVertical();
                Target.Text = GUILayout.TextArea(Target.Text, GUILayout.Height(100));

                if (GUILayout.Button("Add Branch"))
                {
                    IKnob knob = AddKnob(Guid.NewGuid().ToString(), NodeSide.Right, 1, Direction.Output, typeof(string));
                    Target.AddBranch<SimpleBranch>(ID = knob.ID);
                    IBranch branch = Target.Branches[knob.ID];
                    branch.Text = "";
                }

                foreach (var knob in Knobs.Where(x => x.Value.Direction == Direction.Output))
                {
                    GUILayout.BeginHorizontal();
                    Target.Branches[knob.Key].Text = GUILayout.TextField(Target.Branches[knob.Key].Text);
                    DrawKnob(knob.Key);
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this branch?", "Yup", "NO!"))
                            _RemoveKnob = knob.Key;
                    GUILayout.EndHorizontal();
                }

                if (!string.IsNullOrEmpty(_RemoveKnob))
                {
                    RemoveKnob(_RemoveKnob);
                    Target.RemoveBranch(_RemoveKnob);
                    _RemoveKnob = null;
                }

                GUILayout.Space(20);

                MetadataEditor.Editor(Target);

                GUILayout.EndVertical();
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Name = "Event Node")]
        class EventNodeCanvasNode : Node
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Event Node"; } }

            public EventNode Target { get; }

            public EventNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos)
            {
                Target = new EventNode();
                Target.Message = "";
            }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
                AddKnob("out", NodeSide.Right, 1, Direction.Output, typeof(string)).Removable = false;
            }

            public override void OnGUI()
            {

                DrawKnob("in", 20);
                DrawKnob("out", 20);

                Target.Message = GUILayout.TextField(Target.Message);

                GUILayout.Space(20);

                MetadataEditor.Editor(Target);
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Name = "Wait Node")]
        class WaitNodeCanvasNode : Node
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Wait Node"; } }

            public WaitNode Target { get; }

            public WaitNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos)
            {
                Target = new WaitNode();
                Target.Seconds = 0f;
            }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
                AddKnob("out", NodeSide.Right, 1, Direction.Output, typeof(string)).Removable = false;
            }

            public override void OnGUI()
            {
                DrawKnob("in", 20);
                DrawKnob("out", 20);

                Target.Seconds = EditorGUILayout.FloatField(Target.Seconds);

                GUILayout.Space(20);

                MetadataEditor.Editor(Target);
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Name = "End Node")]
        class EndNodeCanvasNode : Node
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "End Node"; } }

            public EndNode Target { get; }

            public EndNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos)
            {
                Target = new EndNode();
            }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
            }

            public override void OnGUI()
            {
                DrawKnob("in", 20);
                MetadataEditor.Editor(Target);
            }
        }

        public class NodeDialogueEditor : EditorWindow
        {
            private static NodeDialogueEditor _editor;
            public static NodeDialogueEditor editor { get { AssureEditor(); return _editor; } }
            public static void AssureEditor() { if (_editor == null) OpenEditor(); }

            private DialogueCanvas Canvas;

            [MenuItem("Window/Ashogue/Dialogue Editor")]
            public static NodeDialogueEditor OpenEditor()
            {
                _editor = EditorWindow.GetWindow<NodeDialogueEditor>();
                _editor.titleContent = new GUIContent("Dialogue Editor");

                return _editor;
            }

            private void Setup()
            {
                Canvas = new DialogueCanvas();
                Canvas.Repaint += Repaint;
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