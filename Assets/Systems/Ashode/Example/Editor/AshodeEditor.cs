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

        state.Nodes.AddRange(new List<Ashode.INode> {
            new SimpleNode { Rect = new Rect(30, 30, 200, 100) },
            new SimpleNode { Rect = new Rect(300, 30, 200, 100) }
        });

        state.Nodes[0].AddKnob<string>("out1", Ashode.NodeSide.Top);
        state.Nodes[0].AddKnob<int>("out2", Ashode.NodeSide.Right);
        state.Nodes[0].AddKnob<float>("out3", Ashode.NodeSide.Bottom);
        state.Nodes[0].AddKnob<bool>("out4", Ashode.NodeSide.Left);

        state.Nodes[1].AddKnob<string>("in1", Ashode.NodeSide.Top);
        state.Nodes[1].AddKnob<int>("in2", Ashode.NodeSide.Left);
        state.Nodes[1].AddKnob<float>("in3", Ashode.NodeSide.Bottom);
        state.Nodes[1].AddKnob<bool>("in4", Ashode.NodeSide.Right);

        state.Connections.AddRange(new List<Ashode.IConnection> {
            new Ashode.Connection<string>
            {
                FromKnob = state.Nodes[0].Knobs["out1"],
                ToKnob = state.Nodes[1].Knobs["in1"]
            },
            new Ashode.Connection<int>
            {
                FromKnob = state.Nodes[0].Knobs["out2"],
                ToKnob = state.Nodes[1].Knobs["in2"]
            },
            new Ashode.Connection<float>
            {
                FromKnob = state.Nodes[0].Knobs["out3"],
                ToKnob = state.Nodes[1].Knobs["in3"]
            },
            new Ashode.Connection<bool>
            {
                FromKnob = state.Nodes[0].Knobs["out4"],
                ToKnob = state.Nodes[1].Knobs["in4"]
            }
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