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
        Ashode.State state = new Ashode.State();

        state.Nodes.AddRange(new List<Ashode.Node> {
            new SimpleNode(),
            // new SimpleNode { Rect = new Rect(230, 30, 200, 100) }
        });

        Canvas = new Ashode.Canvas(state);

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

        Canvas.Draw(GUILayoutUtility.GetRect(600, 600));
    }
}