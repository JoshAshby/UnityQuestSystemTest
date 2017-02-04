using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using System;
using System.Linq;

namespace Ashogue
{
    namespace Editor
    {
        public interface IDialogueNode : Ashode.INode
        {
            Type TargetType { get; }
            Ashogue.Data.INode Target { get; set; }

            IDialogueNode<TNode> OfTargetType<TNode>();
        }

        public interface IDialogueNode<TNode> : Ashode.INode
        {
            TNode Target { get; set; }
        }

        public abstract class DialogueNode<TNode> : Node, IDialogueNode, IDialogueNode<TNode> where TNode : Ashogue.Data.INode
        {
            public DialogueNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

            public Type TargetType { get { return typeof(TNode); } }

            private Ashogue.Data.INode _target;
            public TNode Target
            {
                get { return (TNode)_target; }
                set { _target = (Ashogue.Data.INode)value; }
            }

            Ashogue.Data.INode IDialogueNode.Target
            {
                get { return _target; }
                set { _target = value; }
            }

            TNode IDialogueNode<TNode>.Target
            {
                get { return (TNode)_target; }
                set { _target = (Ashogue.Data.INode)value; }
            }

            public IDialogueNode<TResult> OfTargetType<TResult>()
            {
                if (this is IDialogueNode<TResult>)
                    return (IDialogueNode<TResult>)this;
                else
                    return null;
            }

            public override void Update()
            {
                Target.Position = Rect.position;
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
        class TextNodeCanvasNode : DialogueNode<TextNode>
        {
            public override Vector2 MinSize { get { return new Vector2(200, 100); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Text Node"; } }

            public TextNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
            }

            private string _RemoveKnobID = null;
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
                }

                foreach (var knob in Knobs.Where(x => x.Value.Direction == Direction.Output))
                {
                    GUILayout.BeginHorizontal();
                    Target.Branches[knob.Key].Text = GUILayout.TextField(Target.Branches[knob.Key].Text);
                    DrawKnob(knob.Key);
                    if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                        if (EditorUtility.DisplayDialog("Are you sure?", "Are you sure you want to remove this branch?", "Yup", "NO!"))
                            _RemoveKnobID = knob.Key;
                    GUILayout.EndHorizontal();
                }

                if (!string.IsNullOrEmpty(_RemoveKnobID))
                {
                    RemoveKnob(_RemoveKnobID);
                    Target.RemoveBranch(_RemoveKnobID);
                    _RemoveKnobID = null;
                }

                GUILayout.Space(20);

                MetadataEditor.Editor(Target);

                GUILayout.EndVertical();
            }
        }

        [NodeBelongsTo(typeof(DialogueCanvas), Name = "Event Node")]
        class EventNodeCanvasNode : DialogueNode<EventNode>
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Event Node"; } }

            public EventNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
                AddKnob("out", NodeSide.Right, 0, Direction.Output, typeof(string)).Removable = false;
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
        class WaitNodeCanvasNode : DialogueNode<WaitNode>
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "Wait Node"; } }

            public WaitNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

            public override void SetupKnobs()
            {
                AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
                AddKnob("out", NodeSide.Right, 0, Direction.Output, typeof(string)).Removable = false;
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
        class EndNodeCanvasNode : DialogueNode<EndNode>
        {
            public override Vector2 MinSize { get { return new Vector2(200, 40); } }
            public override bool CanResize { get { return true; } }

            public override string Title { get { return "End Node"; } }

            public EndNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

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
    }
}