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

        IState state = new State(Canvas);
        Canvas.State = state;

        INode a = state.AddNode(typeof(SimpleNode), new Vector2(50, 50));
        a.AddKnob("a", NodeSide.Right, 0, Direction.Both, typeof(string));

        INode b = state.AddNode(typeof(SimpleNode), new Vector2(500, 50));
        b.AddKnob("a", NodeSide.Left, 1, Direction.Both, typeof(string));
        b.AddKnob("b", NodeSide.Bottom, 1, Direction.Input, typeof(int));
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
        if(GUILayout.Button("Add SimpleNode"))
            Canvas.State.AddNode(typeof(SimpleNode), new Vector2(100, 100));
        GUILayout.EndArea();

        Rect canvasRect = new Rect(sideWindowWidth, 0, position.width - sideWindowWidth, position.height);
        Canvas.Draw(canvasRect);
    }
}