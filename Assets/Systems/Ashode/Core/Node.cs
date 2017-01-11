using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Ashode
{
    public enum NodeSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public interface INode
    {
        string ID { get; set; }
        Rect Rect { get; set; }

        string Title { get; set; }

        void DrawNodeWindow(Canvas Canvas);

        void OnGUI();
    }

    public class Knob
    {
        public NodeSide Side = NodeSide.Right;

        private Rect _rect = new Rect(0, 0, 50, 50);
        public Rect Rect {
            get { return _rect; }
            set { _rect = value; }
        }
    }

    public abstract class Node : INode
    {
        public Rect _rect = new Rect(30, 30, 200, 100);
        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        private string _id = Guid.NewGuid().ToString();
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _title = "Window";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public Dictionary<string, Knob> Knobs = new Dictionary<string, Knob>();

        private Vector2 panOffset;
        private Vector2 contentOffset;
        public virtual void DrawNodeWindow(Canvas Canvas)
        {
            Rect nodeRect = Rect;

            nodeRect.position += Canvas.State.PanOffset;
            panOffset = Canvas.State.PanOffset;
            contentOffset = new Vector2(0, 20);

            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Box(headerRect, "", GUI.skin.box);
            GUI.Label(headerRect, Title, Canvas.State.SelectedNode == this ? EditorStyles.boldLabel : EditorStyles.label);

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y + contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);

            GUI.changed = false;
            OnGUI();

            GUILayout.EndArea();
            GUI.EndGroup();

            DrawKnobs();
        }

        public virtual void OnGUI() { }

        void DrawKnobs()
        {
            foreach(var knob in Knobs)
            {
                GUI.Button(knob.Value.Rect, knob.Key);
            }
        }

        public virtual void DrawKnob(string id)
        {
            Vector2 nodePos = Rect.position + panOffset;
            Vector2 position = GUILayoutUtility.GetLastRect().center + contentOffset;
            Knob knob = Knobs[id];

            Rect knobRect = knob.Rect;
            knobRect.position = new Vector2(nodePos.x + Rect.width, nodePos.y + position.y - (knobRect.height/2));

            knob.Rect = knobRect;
            Debug.Log(knobRect.ToString());
        }
    }
}