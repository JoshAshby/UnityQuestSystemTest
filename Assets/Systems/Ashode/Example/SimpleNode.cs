using System.Linq;
using UnityEditor;
using UnityEngine;
using Ashode;

public class SimpleNode : Node
{
    private string id = "";
    public override void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if(GUILayout.Button("Left", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Left);
            if(GUILayout.Button("Right", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Right);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if(GUILayout.Button("Top", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Top);
            if(GUILayout.Button("Bottom", EditorStyles.toolbarButton))
                AddKnob(id, NodeSide.Bottom);
        GUILayout.EndHorizontal();

        string removeKnob = null;
        GUILayout.BeginVertical();
            GUILayout.Label("Simple Node!");
            id = GUILayout.TextField(id);

            foreach(var knob in Knobs.Where(x => x.Value.Side == NodeSide.Left || x.Value.Side == NodeSide.Right))
            {
                GUILayout.BeginHorizontal();
                    GUILayout.Label(knob.Key);
                    DrawKnob(knob.Key);
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
                    if(GUILayout.Button("X"))
                        removeKnob = knob.Key;
                GUILayout.EndHorizontal();
            }

            if(!string.IsNullOrEmpty(removeKnob))
                RemoveKnob(removeKnob);
        GUILayout.EndVertical();

        GUI.Box(new Rect(50, 100, 50, 50), "", GUI.skin.box);
    }
}