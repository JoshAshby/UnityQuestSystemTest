using UnityEditor;
using UnityEngine;
using Ashode;

public class AshodeEditor : EditorWindow
{
    private static AshodeEditor _editor;
    public static AshodeEditor editor { get { AssureEditor(); return _editor; } }
    public static void AssureEditor() { if (_editor == null) OpenEditor(); }

    private NodeCanvas Canvas;

    [MenuItem("Window/Ashode/Example Editor")]
    public static AshodeEditor OpenEditor()
    {
        _editor = EditorWindow.GetWindow<AshodeEditor>();
        _editor.titleContent = new GUIContent("Example Editor");

        return _editor;
    }

    private void Setup()
    {
        Canvas = new NodeCanvas();
        Canvas.Repaint += Repaint;

        State state = new State(Canvas);
        Canvas.State = state;

        INode a = state.AddNode(typeof(SimpleNode), new Rect(50, 50, 200, 100));
        a.AddKnob("a", NodeSide.Right, 0, Direction.Both, typeof(string));

        INode b = state.AddNode(typeof(SimpleNode), new Rect(500, 50, 200, 100));
        b.AddKnob("a", NodeSide.Left, 1, Direction.Both, typeof(string));
        b.AddKnob("b", NodeSide.Bottom, 1, Direction.Input, typeof(string));
        b.AddKnob("d", NodeSide.Bottom, 1, Direction.Input, typeof(int));
        b.AddKnob("c", NodeSide.Left, 1, Direction.Output, typeof(string));

        // INode c = state.AddNode(typeof(SimpleNode), new Rect(500, 400, 200, 100));
        // c.AddKnob("a", NodeSide.Left, 1, Direction.Both, typeof(string));
        // c.AddKnob("b", NodeSide.Top, 1, Direction.Input, typeof(string));
        // c.AddKnob("d", NodeSide.Top, 1, Direction.Input, typeof(int));
        // c.AddKnob("c", NodeSide.Left, 1, Direction.Output, typeof(string));

        // state.AddConnection(a.Knobs["a"], b.Knobs["a"]);
        // state.AddConnection(a.Knobs["a"], c.Knobs["a"]);

        // state.AddConnection(a.Knobs["a"], b.Knobs["b"]);
        // state.AddConnection(a.Knobs["a"], c.Knobs["b"]);

        // state.AddConnection(a.Knobs["a"], b.Knobs["c"]);
        // state.AddConnection(a.Knobs["a"], c.Knobs["c"]);
    }

    private void OnDestroy()
    {
        Canvas.Repaint -= Repaint;
    }

    private void OnGUI()
    {
        if (Canvas == null)
            Setup();

        Canvas.Draw(GUILayoutUtility.GetRect(600, 900));

        Rect sideWindowRect = new Rect(600, 200, 200, 200);

        GUILayout.BeginArea(sideWindowRect, GUI.skin.box);
        GUILayout.Button("test");
        GUILayout.EndArea();
    }
}