using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using Ashode;

public class ExampleCanvas : NodeCanvas
{
    public ExampleCanvas() : base() { }
}

[NodeBelongsTo(typeof(ExampleCanvas), Name = "Simple Node")]
public class SimpleNode : Node
{
    public override Vector2 MinSize { get { return new Vector2(200, 100); } }
    public override bool CanResize { get { return true; } }

    public override string Title { get { return "SimpleNode"; } }

    public SimpleNode(INodeCanvas parent, Vector2 pos) : base(parent, pos) { }

    private string id = "";
    private int limit = 1;
    private Direction direction = Direction.Both;
    public override void OnGUI()
    {
        GUILayout.BeginVertical();

        string removeKnob = null;
        GUILayout.BeginVertical();
        id = GUILayout.TextField(id);

        limit = EditorGUILayout.IntField("Limit", limit);
        if (limit < 0)
            limit = 0;

        direction = (Direction)EditorGUILayout.EnumPopup("Direction", direction);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Top"))
            AddKnob<string>(id, NodeSide.Top, limit, direction);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Left"))
            AddKnob<string>(id, NodeSide.Left, limit, direction);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Right"))
            AddKnob<string>(id, NodeSide.Right, limit, direction);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Bottom"))
            AddKnob<string>(id, NodeSide.Bottom, limit, direction);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        foreach (var knob in Knobs.Where(x => x.Value.Side == NodeSide.Left || x.Value.Side == NodeSide.Right))
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(knob.Key);
            DrawKnob(knob.Key);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X"))
                removeKnob = knob.Key;
            GUILayout.EndHorizontal();
        }

        var topKnobs = Knobs.Where(x => x.Value.Side == NodeSide.Top).ToList();
        for (int i = 0; i < topKnobs.Count; i++)
        {
            var knob = topKnobs[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(knob.Key);
            DrawKnob(knob.Key, i * 25);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X"))
                removeKnob = knob.Key;
            GUILayout.EndHorizontal();
        }

        var bottomKnobs = Knobs.Where(x => x.Value.Side == NodeSide.Bottom).ToList();
        for (int i = 0; i < bottomKnobs.Count; i++)
        {
            var knob = bottomKnobs[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label(knob.Key);
            DrawKnob(knob.Key, i * 25);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X"))
                removeKnob = knob.Key;
            GUILayout.EndHorizontal();
        }

        if (!string.IsNullOrEmpty(removeKnob))
            RemoveKnob(removeKnob);
        GUILayout.EndVertical();
    }
}

public class AshodeEditor : EditorWindow
{
    private static AshodeEditor _editor;
    public static AshodeEditor editor { get { AssureEditor(); return _editor; } }
    public static void AssureEditor() { if (_editor == null) OpenEditor(); }

    private ExampleCanvas Canvas;

    [MenuItem("Window/Ashode/Example Editor")]
    public static AshodeEditor OpenEditor()
    {
        _editor = EditorWindow.GetWindow<AshodeEditor>();
        _editor.titleContent = new GUIContent("Example Editor");

        return _editor;
    }

    private void Setup()
    {
        Canvas = new ExampleCanvas();
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

        Rect canvasRect = new Rect(0, 0, position.width, position.height);
        Canvas.Draw(canvasRect);
    }
}