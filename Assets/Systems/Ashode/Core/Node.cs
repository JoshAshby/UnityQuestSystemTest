using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface INode : IControl
    {
        INodeCanvas Parent { get; }
        INodeCanvas Canvas { get; }
        IState State { get; }

        string ID { get; set; }
        Rect Rect { get; set; }
        Vector2 MinSize { get; }
        bool CanResize { get; }

        string Title { get; }

        void DrawNodeWindow();
        void ResizeWindow();

        void SetupKnobs();
        void OnGUI();

        Dictionary<string, IKnob> Knobs { get; set; }

        void DrawKnobWindows();
        void DrawKnob(string id);
        void DrawKnob(string id, float position);

        IKnob AddKnob(string id, NodeSide side, int limit, Direction direction, Type TAccept);
        IKnob AddKnob<TAccept>(string id, NodeSide side, int limit, Direction direction);
        void RemoveKnob(string id);
    }

    public abstract class Node : INode
    {
        public INodeCanvas Parent { get; internal set; }
        public INodeCanvas Canvas { get { return Parent; } }
        public IState State { get { return Canvas.State; } }

        private string _id = Guid.NewGuid().ToString();
        public virtual string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public virtual Rect Rect { get; set; }

        private Vector2 _minSize = new Vector2(200, 100);
        public virtual Vector2 MinSize
        {
            get { return _minSize; }
            set { _minSize = value; }
        }

        private bool _canResize = true;
        public virtual bool CanResize
        {
            get { return _canResize; }
            set { _canResize = value; }
        }

        private string _title = "Window";
        public virtual string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private Dictionary<string, IKnob> _knobs = new Dictionary<string, IKnob>();
        public Dictionary<string, IKnob> Knobs
        {
            get { return _knobs; }
            set { _knobs = value; }
        }

        public Node(INodeCanvas parent, Vector2 pos)
        {
            this.Parent = parent;
            this.Rect = new Rect(pos.x, pos.y, MinSize.x, MinSize.y);

            SetupKnobs();
        }

        public virtual void SetupKnobs() { }

        private Vector2 lastPosition;
        private Vector2 contentOffset = new Vector2(0, 20);
        public virtual void DrawNodeWindow()
        {
            if (lastPosition == null)
                lastPosition = Rect.max;

            Rect nodeRect = Rect;

            nodeRect.position += Canvas.State.PanOffset;

            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Box(headerRect, "", GUI.skin.box);
            GUI.Label(headerRect, Title, Canvas.State.SelectedNode == this ? EditorStyles.boldLabel : EditorStyles.label);

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y + contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);

            GUI.changed = false;
            OnGUI();

            // https://docs.unity3d.com/ScriptReference/GUILayoutUtility.GetLastRect.html
            // GetLastRect only works during repaint events
            if (Event.current.type == EventType.Repaint)
                lastPosition = GUILayoutUtility.GetLastRect().max + contentOffset;

            GUILayout.EndArea();
            GUI.EndGroup();

            DrawKnobWindows();
            ResizeWindow();
        }

        public virtual void ResizeWindow()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (!CanResize)
                return;

            Rect nodeRect = Rect;

            Vector2 maxSize = lastPosition + contentOffset;

            // TODO: Think about handling manual resizes too
            List<IKnob> topBottomKnobs = Knobs.Values.Where(x => x.Side == NodeSide.Bottom || x.Side == NodeSide.Top).ToList();
            if (topBottomKnobs.Any())
            {
                float knobSize = topBottomKnobs.Max(x => x.Rect.xMax - nodeRect.xMin);
                float minWidth = MinSize.x;

                maxSize.x = new List<float> { knobSize, minWidth }.Max();
            }
            else
            {
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

        public virtual void DrawKnobWindows()
        {
            foreach (var knob in Knobs.Values)
                knob.DrawKnobWindow();
        }

        public virtual void OnGUI() { }

        public virtual void DrawKnob(string id)
        {
            // https://docs.unity3d.com/ScriptReference/GUILayoutUtility.GetLastRect.html
            // GetLastRect only works during repaint events
            if (Event.current.type != EventType.Repaint)
                return;

            Vector2 position = GUILayoutUtility.GetLastRect().center + contentOffset;

            IKnob knob = Knobs[id];

            DrawKnob(id, knob.Side == NodeSide.Top || knob.Side == NodeSide.Bottom ? position.x : position.y);
        }

        public virtual void DrawKnob(string id, float position)
        {
            Rect nodeRect = Rect;
            Vector2 nodePos = nodeRect.position;

            IKnob knob = Knobs[id];
            Rect knobRect = knob.Rect;

            switch (knob.Side)
            {
                case NodeSide.Left:
                    knobRect.position = new Vector2(nodePos.x - knobRect.width - knob.Offset, nodePos.y + position - (knobRect.height / 2));
                    break;

                case NodeSide.Right:
                    knobRect.position = new Vector2(nodeRect.xMax + knob.Offset, nodePos.y + position - (knobRect.height / 2));
                    break;

                case NodeSide.Top:
                    knobRect.position = new Vector2(nodePos.x + position, nodePos.y - knobRect.height - knob.Offset);
                    break;

                case NodeSide.Bottom:
                    knobRect.position = new Vector2(nodePos.x + position, nodeRect.yMax + knob.Offset);
                    break;
            }

            knob.Rect = knobRect;
        }

        public IKnob AddKnob(string id, NodeSide side, int limit, Direction direction, Type TAccept)
        {
            IKnob knob = new Knob
            {
                ID = id,
                ConnectionLimit = limit,
                Side = side,
                Direction = direction,
                Type = TAccept,
                Parent = this
            };

            Knobs.Add(id, knob);
            return knob;
        }

        public IKnob AddKnob<TAccept>(string id, NodeSide side, int limit, Direction direction)
        {
            return AddKnob(id, side, limit, direction, typeof(TAccept));
        }

        public void RemoveKnob(string id)
        {
            IKnob knob = Knobs[id];

            // TODO: Throw an error or debug at least if removing a non-removable knob?
            if (!knob.Removable)
            {
                Debug.LogErrorFormat("Can't remove a non-removable knob: {0}", id);
                return;
            }

            if (State.FocusedKnob == knob)
                State.FocusedKnob = null;

            if (State.SelectedKnob == knob)
                State.SelectedNode = null;

            if (State.ExpandedKnob == knob)
                State.ExpandedKnob = null;

            if (State.ConnectedFromKnob == knob)
                State.ConnectedFromKnob = null;

            foreach (var conn in Knobs[id].Connections)
                conn.Parent.State.RemoveConnection(conn);

            Knobs.Remove(id);
        }

        public bool HitTest(Vector2 loc, out IControl hit)
        {
            hit = null;

            if (Rect.Contains(loc))
            {
                hit = this;
                return true;
            }

            foreach (var knob in Knobs.Values)
                if (knob.HitTest(loc, out hit))
                    return true;

            return false;
        }
    }
}