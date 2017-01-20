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
            new SimpleNode(state, new Rect(30, 30, 200, 100)),
            new SimpleNode(state, new Rect(300, 30, 200, 100))
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