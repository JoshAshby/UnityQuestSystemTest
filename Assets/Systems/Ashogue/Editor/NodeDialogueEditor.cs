using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Ashogue;
using Ashogue.Data;
using Ashogue.Extensions;

namespace Ashode
{
    [AttributeUsage(AttributeTargets.Class)]
    class NodeAttribute : Attribute
    {
        public string Name { get; set; }
        public bool Single { get; set; }

        public NodeAttribute(string name)
        {
            this.Name = name;
        }

        public static Type[] GetTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttributes(typeof(NodeAttribute), false).Any() )
                .OrderBy(x => x.Name)
                .ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    class ConnectionAttribute : Attribute
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public ConnectionAttribute(string name)
        {
            this.Name = name;
        }

        public static Type[] GetTypes()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.GetCustomAttributes(typeof(ConnectionAttribute), false).Any() )
                .OrderBy(x => x.Name)
                .ToArray();
        }
    }

    public enum NodeSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum DrawStyle
    {
        Straight,
        Curve
    }

    public class Node
    {
        public string ID { get; }
        public object target { get; set; }

        public virtual void SetupConnections() { }
        public virtual void OnGUI() { }

        public virtual void Calculate() { }

        public void Knob<T>(string name, NodeSide side) { }
    }

    public class Node<T> : Node
    {
        public new T target { get; set; }
    }

    public class Connection<T>
    {
        public string ID { get; }

        public Type Type { get { return typeof(T); } }
        public T Value { get; }

        public Color Color { get; }
        public DrawStyle Style { get; }
    }

    // =========================================================================

    [Connection("Node ID Connection")]
    class IDConnection : Connection<string>
    {
        public new Color Color { get { return Color.red; } }
        public new DrawStyle Style { get { return DrawStyle.Curve; } }
    }

    [Node("Start Node", Single = true)]
    class StartNode : Ashode.Node
    {
        public override void OnGUI()
        {
            Knob<IDConnection>("Start Node", NodeSide.Right);
        }
    }

    [Ashode.Node("Text Node")]
    class TextNode : Ashode.Node<Ashogue.Data.TextNode>
    {
        public override void OnGUI()
        {
            Knob<IDConnection>("", NodeSide.Right);

            if (GUILayout.Button("Add Branch"))
                ((IBranchedNode)target).AddBranch<SimpleBranch>();

            foreach (IBranch branch in ((IBranchedNode)target).Branches.Values)
            {
                Knob<IDConnection>(branch.ID, NodeSide.Right);
            }
        }
    }

    [NodeAttribute("End Node")]
    class EndNode : Node<Ashogue.Data.EndNode>
    {
        public override void OnGUI()
        {
            Knob<IDConnection>("End", NodeSide.Left);
        }

        public override void Calculate()
        {

        }
    }
}

public class NodeDialogueEditor : EditorWindow
{
    private DialogueContainer Container;
    private string ContainerPath;

    private static NodeDialogueEditor _editor;
    public static NodeDialogueEditor editor { get { AssureEditor(); return _editor; } }
    public static void AssureEditor() { if (_editor == null) OpenEditor(); }

    [MenuItem("Window/Ashogue/Dialogue Editor")]
    public static NodeDialogueEditor OpenEditor()
    {
        _editor = EditorWindow.GetWindow<NodeDialogueEditor>();

        _editor.titleContent = new GUIContent("Dialogue Editor");

        return _editor;
    }

    private void OnGUI()
    {

    }
}