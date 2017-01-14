using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface INode
    {
        string ID { get; set; }
        Vector2 MinSize { get; set; }
        Rect Rect { get; set; }

        string Title { get; set; }

        void DrawNodeWindow(Canvas Canvas);
        void ResizeWindow(Canvas Canvas);

        void OnGUI();

        void DrawKnobWindows(Canvas Canvas);
        void DrawKnob(string id);
        void DrawKnob(string id, float position);

        Knob AddKnob(string id, NodeSide side);
        void RemoveKnob(string id);
    }

    public abstract class Node : INode
    {
        private Vector2 _minSize = new Vector2(200, 100);
        public Vector2 MinSize
        {
            get { return _minSize; }
            set { _minSize = value; }
        }

        private Rect _rect = new Rect(30, 30, 200, 100);
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

        private Vector2 lastPosition;
        private Vector2 panOffset;
        private Vector2 contentOffset;
        public virtual void DrawNodeWindow(Canvas Canvas)
        {
            if (lastPosition == null)
                lastPosition = Rect.max;

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
            lastPosition = GUILayoutUtility.GetLastRect().max + contentOffset;

            GUILayout.EndArea();
            GUI.EndGroup();

            DrawKnobWindows(Canvas);
            ResizeWindow(Canvas);
        }
        
        public virtual void ResizeWindow(Canvas Canvas)
        {
            Rect nodeRect = Rect;

            if (Event.current.type != EventType.Repaint)
                return;

            Vector2 maxSize = lastPosition + contentOffset;

            // TODO: Think about handling manual resizes too
            if(Knobs.Values.Where(x => x.Side == NodeSide.Bottom || x.Side == NodeSide.Top).Any())
            {
                float topSize = Knobs.Max(x => x.Value.Side == NodeSide.Top ? x.Value.Rect.xMax : 0);
                float bottomSize = Knobs.Max(x => x.Value.Side == NodeSide.Bottom ? x.Value.Rect.xMax : 0);
                float minWidth = MinSize.x;

                maxSize.x = new List<float>{ topSize, bottomSize, minWidth }.Max();
            } else {
                maxSize.x = nodeRect.width;
            }

            if (maxSize != nodeRect.size)
                nodeRect.size = maxSize;

            if (Rect.size != nodeRect.size)
            {
                Rect = nodeRect;
                Canvas.OnRepaint();
            }
        }

        public virtual void DrawKnobWindows(Canvas Canvas)
        {
            foreach (var knob in Knobs.Values)
            {
                knob.DrawKnobWindow(Canvas);
            }
        }

        public virtual void OnGUI() { }

        public virtual void DrawKnob(string id)
        {
            Rect nodeRect = Rect;
            nodeRect.position += panOffset;

            Vector2 nodePos = nodeRect.position;
            Vector2 position = GUILayoutUtility.GetLastRect().center + contentOffset;

            Knob knob = Knobs[id];

            Rect knobRect = knob.Rect;
            switch (knob.Side)
            {
                case NodeSide.Right:
                    knobRect.position = new Vector2(nodePos.x + Rect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;

                case NodeSide.Left:
                    knobRect.position = new Vector2(nodePos.x - knobRect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;

                case NodeSide.Top:
                    knobRect.position = new Vector2(nodePos.x - knobRect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;

                case NodeSide.Bottom:
                    knobRect.position = new Vector2(nodePos.x - knobRect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;
            }

            knob.Rect = knobRect;
        }

        public virtual void DrawKnob(string id, float position)
        {
            Rect nodeRect = Rect;
            nodeRect.position += panOffset;

            Vector2 nodePos = nodeRect.position;

            Knob knob = Knobs[id];

            Rect knobRect = knob.Rect;
            switch (knob.Side)
            {
                case NodeSide.Top:
                    knobRect.position = new Vector2(nodePos.x + position, nodePos.y - knobRect.height);
                    break;

                case NodeSide.Bottom:
                    knobRect.position = new Vector2(nodePos.x + position, nodeRect.yMax);
                    break;
            }

            knob.Rect = knobRect;
        }

        public Knob AddKnob(string id, NodeSide side)
        {
            Knob knob = new Knob { ID = id, Side = side };
            Knobs.Add(id, knob);
            return knob;
        }

        public void RemoveKnob(string id)
        {
            Knobs.Remove(id);
        }
    }
}