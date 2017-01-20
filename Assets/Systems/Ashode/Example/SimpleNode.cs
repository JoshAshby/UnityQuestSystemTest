using System.Linq;
using UnityEditor;
using UnityEngine;
using Ashode;

public class SimpleNode : Node
{
    private string id = "";

    bool allowMultiple = false;
    Direction direction = Direction.Both;

    public SimpleNode(State state, Rect rect) : base(state)
    {
        this.Rect = rect;
    }

    public override void OnGUI()
    {
        GUILayout.BeginVertical();
        allowMultiple = GUILayout.Toggle(allowMultiple, "Allow Multiple?");
        direction = (Direction) EditorGUILayout.EnumPopup("Direction", direction);
        GUILayout.EndVertical();

        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Top"))
                AddKnob<object>(id, NodeSide.Top, allowMultiple, direction);
            GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            if(GUILayout.Button("Left"))
                AddKnob<string>(id, NodeSide.Left, allowMultiple, direction);
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Right"))
                AddKnob<int>(id, NodeSide.Right, allowMultiple, direction);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if(GUILayout.Button("Bottom"))
                AddKnob<float>(id, NodeSide.Bottom, allowMultiple, direction);
            GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(20);

        string removeKnob = null;
        GUILayout.BeginVertical();
            GUILayout.Label("Simple Node!");
            id = GUILayout.TextField(id);

            foreach(var knob in Knobs.Where(x => x.Value.Side == NodeSide.Left || x.Value.Side == NodeSide.Right))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label(knob.Key);
                    DrawKnob(knob.Key);
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("X"))
                        removeKnob = knob.Key;
                GUILayout.EndHorizontal();
            }

            var topKnobs = Knobs.Where(x => x.Value.Side == NodeSide.Top).ToList();
            for(int i = 0; i < topKnobs.Count; i++)
            {
                var knob = topKnobs[i];

                GUILayout.BeginHorizontal();
                    GUILayout.Label(knob.Key);
                    DrawKnob(knob.Key, i * 25);
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("X"))
                        removeKnob = knob.Key;
                GUILayout.EndHorizontal();
            }

            var bottomKnobs = Knobs.Where(x => x.Value.Side == NodeSide.Bottom).ToList();
            for(int i = 0; i < bottomKnobs.Count; i++)
            {
                var knob = bottomKnobs[i];

                GUILayout.BeginHorizontal();
                    GUILayout.Label(knob.Key);
                    DrawKnob(knob.Key, i * 25);
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button("X"))
                        removeKnob = knob.Key;
                GUILayout.EndHorizontal();
            }

            if(!string.IsNullOrEmpty(removeKnob))
                RemoveKnob(removeKnob);
        GUILayout.EndVertical();
    }
}