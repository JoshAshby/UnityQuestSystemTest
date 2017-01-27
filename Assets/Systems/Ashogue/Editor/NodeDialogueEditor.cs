using UnityEditor;
using UnityEngine;
using Ashode;
using Ashogue.Data;
using System;

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
        menu.AddItem(new GUIContent("Add/Text Node"), false, AddNodeCallback, new CallbackEvent<Type>(inputEvent, typeof(TextNodeCanvasNode)));
        menu.AddItem(new GUIContent("Add/Event Node"), false, AddNodeCallback, new CallbackEvent<Type>(inputEvent, typeof(EventNodeCanvasNode)));
        menu.AddItem(new GUIContent("Add/Wait Node"), false, AddNodeCallback, new CallbackEvent<Type>(inputEvent, typeof(WaitNodeCanvasNode)));
        menu.AddItem(new GUIContent("Add/End Node"), false, AddNodeCallback, new CallbackEvent<Type>(inputEvent, typeof(EndNodeCanvasNode)));
        menu.ShowAsContext();

        inputEvent.Event.Use();
    }
}

class DialogueCanvas : NodeCanvas
{
    public DialogueCanvas() : base() { }
}

class StartNodeCanvasNode : Node
{
    private Vector2 _minSize = new Vector2(200, 100);
    public override Vector2 MinSize { get { return _minSize; } set { _minSize = value; } }

    private string _title = "Text Node";
    public override string Title { get { return _title; } set { _title = value; } }

    public StartNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void SetupKnobs()
    {
        AddKnob("start", NodeSide.Right, 1, Direction.Output, typeof(string));
    }

    public override void OnGUI()
    {

    }
}

class TextNodeCanvasNode : Node
{
    private Vector2 _minSize = new Vector2(200, 100);
    public override Vector2 MinSize { get { return _minSize; } set { _minSize = value; } }

    private string _title = "Text Node";
    public override string Title { get { return _title; } set { _title = value; } }

    public TextNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void OnGUI()
    {

    }
}

class EventNodeCanvasNode : Node
{
    private Vector2 _minSize = new Vector2(200, 100);
    public override Vector2 MinSize { get { return _minSize; } set { _minSize = value; } }

    private string _title = "Event Node";
    public override string Title { get { return _title; } set { _title = value; } }

    public EventNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void OnGUI()
    {

    }
}

class WaitNodeCanvasNode : Node
{
    private Vector2 _minSize = new Vector2(200, 100);
    public override Vector2 MinSize { get { return _minSize; } set { _minSize = value; } }

    private string _title = "Wait Node";
    public override string Title { get { return _title; } set { _title = value; } }

    public WaitNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void OnGUI()
    {

    }
}

class EndNodeCanvasNode : Node
{
    private Vector2 _minSize = new Vector2(200, 100);
    public override Vector2 MinSize { get { return _minSize; } set { _minSize = value; } }

    private string _title = "End Node";
    public override string Title { get { return _title; } set { _title = value; } }

    public EndNodeCanvasNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    public override void OnGUI()
    {

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

        Canvas.State.AddNode(typeof(StartNodeCanvasNode), new Vector2(0, 0));
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