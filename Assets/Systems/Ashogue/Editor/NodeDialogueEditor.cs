using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

class CallbackEvent<T>
{
    public InputEvent InputEvent { get; }
    public T Object { get; }

    public Vector2 CanvasSpaceMouse
    {
        get { return InputEvent.Canvas.ScreenToCanvasSpace((InputEvent.Event.mousePosition)) - InputEvent.State.GlobalCanvasSize.position; }
    }

    public CallbackEvent(InputEvent inputEvent, T action)
    {
        this.InputEvent = inputEvent;
        this.Object = action;
    }
}

static class ContextMenuHanders
{
    private static void AddNodeCallback(object obj)
    {
        CallbackEvent<Type> callbackEvent = obj as CallbackEvent<Type>;

        callbackEvent.InputEvent.State.AddNode(callbackEvent.Object, callbackEvent.CanvasSpaceMouse);
    }

    // Right click on canvas
    [MouseEventHandler(MouseButtons.Right, Priority = 2)]
    public static void HandleCanvasRightClick(InputEvent inputEvent)
    {
        if (inputEvent.Control != null)
            return;

        GenericMenu menu = new GenericMenu();
        foreach(var attr in inputEvent.Canvas.NodeTypes())
        {
            menu.AddItem(new GUIContent(String.Format("Add/{0}", attr.Name)), false, AddNodeCallback, new CallbackEvent<Type>(inputEvent, attr.NodeType));
        }
        menu.ShowAsContext();

        inputEvent.Event.Use();
    }
}

class DialogueCanvas : NodeCanvas
{
    public DialogueCanvas() : base() { }
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

    public TextNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void SetupKnobs()
    {
        AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
    }

    Dictionary<string, string> _KnobDisplayText = new Dictionary<string, string>();
    string _Text = "";

    private string _RemoveKnob = null;
    public override void OnGUI()
    {
        DrawKnob("in", 20);

        GUILayout.BeginVertical();
        _Text = GUILayout.TextArea(_Text, GUILayout.Height(100));

        if (GUILayout.Button("Add Branch"))
        {
            IKnob knob = AddKnob(Guid.NewGuid().ToString(), NodeSide.Right, 1, Direction.Output, typeof(string));
            _KnobDisplayText.Add(knob.ID, "");
        }

        foreach (var knob in Knobs.Where(x => x.Value.Direction == Direction.Output))
        {
            GUILayout.BeginHorizontal();
            _KnobDisplayText[knob.Key] = GUILayout.TextField(_KnobDisplayText[knob.Key]);
            DrawKnob(knob.Key);
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                _RemoveKnob = knob.Key;
            GUILayout.EndHorizontal();
        }

        if (!string.IsNullOrEmpty(_RemoveKnob))
        {
            RemoveKnob(_RemoveKnob);
            _KnobDisplayText.Remove(_RemoveKnob);
            _RemoveKnob = null;
        }

        GUILayout.EndVertical();
    }
}

[NodeBelongsTo(typeof(DialogueCanvas), Name = "Event Node")]
class EventNodeCanvasNode : Node
{
    public override Vector2 MinSize { get { return new Vector2(100, 40); } }
    public override bool CanResize { get { return true; } }

    public override string Title { get { return "Event Node"; } }

    public EventNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void SetupKnobs()
    {
        AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
        AddKnob("out", NodeSide.Right, 1, Direction.Output, typeof(string)).Removable = false;
    }

    string _Msg = "";
    public override void OnGUI()
    {

        DrawKnob("in", 20);
        DrawKnob("out", 20);

        _Msg = GUILayout.TextField(_Msg);
    }
}

[NodeBelongsTo(typeof(DialogueCanvas), Name = "Wait Node")]
class WaitNodeCanvasNode : Node
{
    public override Vector2 MinSize { get { return new Vector2(100, 40); } }
    public override bool CanResize { get { return true; } }

    public override string Title { get { return "Wait Node"; } }

    public WaitNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void SetupKnobs()
    {
        AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
        AddKnob("out", NodeSide.Right, 1, Direction.Output, typeof(string)).Removable = false;
    }

    int Wait = 0;
    public override void OnGUI()
    {
        DrawKnob("in", 20);
        DrawKnob("out", 20);

        Wait = EditorGUILayout.IntField(Wait);
    }
}

[NodeBelongsTo(typeof(DialogueCanvas), Name = "End Node")]
class EndNodeCanvasNode : Node
{
    public override Vector2 MinSize { get { return new Vector2(100, 40); } }
    public override bool CanResize { get { return false; } }

    public override string Title { get { return "End Node"; } }

    public EndNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void SetupKnobs()
    {
        AddKnob("in", NodeSide.Left, 0, Direction.Input, typeof(string)).Removable = false;
    }

    public override void OnGUI()
    {
        DrawKnob("in", 20);
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

        Canvas.InputSystem.AddHandlersFrom(typeof(ContextMenuHanders));

        Canvas.State.AddNode(typeof(StartNodeCanvasNode), new Vector2(10, 10));
    }

    private void OnDestroy()
    {
        Canvas.Repaint -= Repaint;
    }

    private void OnGUI()
    {
        if (Canvas == null)
            Setup();

        int sideWindowWidth = 200;

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