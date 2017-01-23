using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AshodeEditor : EditorWindow
{
    private static AshodeEditor _editor;
    public static AshodeEditor editor { get { AssureEditor(); return _editor; } }
    public static void AssureEditor() { if (_editor == null) OpenEditor(); }

    private Ashode.Canvas Canvas;

    [MenuItem("Window/Ashode/Example Editor")]
    public static AshodeEditor OpenEditor()
    {
        _editor = EditorWindow.GetWindow<AshodeEditor>();
        _editor.titleContent = new GUIContent("Example Editor");

        return _editor;
    }

    private void Setup()
    {
        Canvas = new Ashode.Canvas();
        Canvas.Repaint += Repaint;

        Ashode.State state = new Ashode.State(Canvas);
        Canvas.State = state;

        state.Nodes.AddRange(new List<Ashode.INode> {
            new SimpleNode(state, new Rect(50, 50, 200, 100)),
            new SimpleNode(state, new Rect(500, 50, 200, 100)),
            new SimpleNode(state, new Rect(500, 300, 200, 100))
        });

        Ashode.IKnob a = state.Nodes[0].AddKnob("a", Ashode.NodeSide.Right, true, Ashode.Direction.Both, typeof(string));
        Ashode.IKnob b = state.Nodes[1].AddKnob("b", Ashode.NodeSide.Left, true, Ashode.Direction.Both, typeof(string));
        Ashode.IKnob c = state.Nodes[2].AddKnob("c", Ashode.NodeSide.Left, true, Ashode.Direction.Both, typeof(string));

        state.Connections.AddRange(new List<Ashode.IConnection> {
            new Ashode.Connection(Canvas, a, b),
            new Ashode.Connection(Canvas, a, c)
        });
    }

    private void OnDestroy()
    {
        Canvas.Repaint -= Repaint;
    }

    private void OnGUI()
    {
        if (Canvas == null)
            Setup();

        Canvas.Draw(GUILayoutUtility.GetRect(600, 600));
    }
}