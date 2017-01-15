using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface INode
    {
        string ID { get; set; }
        Rect Rect { get; set; }
        Vector2 MinSize { get; set; }
        bool CanResize { get; set; }

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

        private bool _canResize = true;
        public bool CanResize
        {
            get { return _canResize; }
            set { _canResize = value; }
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

        private Dictionary<string, Knob> _knobs = new Dictionary<string, Knob>();
        public Dictionary<string, Knob> Knobs
        {
            get { return _knobs; }
            set { _knobs = value; }
        }

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
            if (Event.current.type != EventType.Repaint)
                return;

            if (!CanResize)
                return;

            Rect nodeRect = Rect;
            nodeRect.position += panOffset;

            Vector2 maxSize = lastPosition + contentOffset;

            // TODO: Think about handling manual resizes too
            List<Knob> topBottomKnobs = Knobs.Values.Where(x => x.Side == NodeSide.Bottom || x.Side == NodeSide.Top).ToList();
            if(topBottomKnobs.Any())
            {
                float knobSize = topBottomKnobs.Max(x => x.Rect.xMax - nodeRect.xMin);
                float minWidth = MinSize.x;

                maxSize.x = new List<float>{ knobSize, minWidth }.Max() - panOffset.x;
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
            Vector2 position = GUILayoutUtility.GetLastRect().center + contentOffset;

            Rect nodeRect = Rect;
            Vector2 nodePos = nodeRect.position;
            nodePos += panOffset;

            Knob knob = Knobs[id];
            Rect knobRect = knob.Rect;

            switch (knob.Side)
            {
                case NodeSide.Left:
                    knobRect.position = new Vector2(nodePos.x - knobRect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;

                case NodeSide.Right:
                    knobRect.position = new Vector2(nodePos.x + nodeRect.width, nodePos.y + position.y - (knobRect.height / 2));
                    break;

                case NodeSide.Top:
                    knobRect.position = new Vector2(nodePos.x + position.x, nodePos.y - knobRect.height);
                    break;

                case NodeSide.Bottom:
                    knobRect.position = new Vector2(nodePos.x + position.x, nodeRect.yMax);
                    break;
            }

            knob.Rect = knobRect;
        }

        public virtual void DrawKnob(string id, float position)
        {
            Rect nodeRect = Rect;
            Vector2 nodePos = nodeRect.position;
            nodePos += panOffset;

            Knob knob = Knobs[id];
            Rect knobRect = knob.Rect;

            switch (knob.Side)
            {
                case NodeSide.Left:
                    knobRect.position = new Vector2(nodePos.x - Rect.width, nodePos.y + position - (knobRect.height / 2));
                    break;

                case NodeSide.Right:
                    knobRect.position = new Vector2(nodePos.x + Rect.width, nodePos.y + position - (knobRect.height / 2));
                    break;

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