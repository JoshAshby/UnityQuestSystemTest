using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public enum NodeSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum Direction
    {
        Input,
        Output,
        Both
    }

    public interface IKnob : IControl
    {
        INode Parent { get; }
        INodeCanvas Canvas { get; }
        State State { get; }

        string ID { get; set; }

        Rect Rect { get; set; }
        float Offset { get; set; }
        bool Expanded { get; }

        NodeSide Side { get; set; }
        Vector3 DirectionVector { get; }

        Direction Direction { get; set; }
        int ConnectionLimit { get; set; }
        bool Removable { get; set; }

        Type Type { get; }
        bool Available { get; }

        List<IConnection> Connections { get; }
        Vector2 CenterForConnection(IConnection conn);

        void DrawKnobWindow();
    }

    public class Knob : IKnob
    {
        public INode Parent { get; internal set; }
        public INodeCanvas Canvas { get { return Parent.Parent; } }
        public State State { get { return Canvas.State; } }

        private string _id = Guid.NewGuid().ToString();
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private Rect _rect = new Rect(0, 0, 20, 20);
        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        private float _offset = 0;
        public float Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }

        public bool Expanded
        {
            get { return Canvas.State.ExpandedKnob == this; }
        }

        private NodeSide _side = NodeSide.Right;
        public NodeSide Side
        {
            get { return _side; }
            set { _side = value; }
        }

        private Direction _direction = Direction.Both;
        public Direction Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public Vector3 DirectionVector
        {
            get
            {
                switch (Side)
                {
                    case NodeSide.Left:
                        return Vector3.left;

                    case NodeSide.Right:
                        return Vector3.right;

                    case NodeSide.Top:
                        return Vector3.down;

                    case NodeSide.Bottom:
                        return Vector3.up;

                    default:
                        return Vector3.right;
                }
            }
        }

        private int _connectionLimit = 1;
        public int ConnectionLimit
        {
            get { return _connectionLimit; }
            set { _connectionLimit = value; }
        }

        private bool _removable = true;
        public bool Removable
        {
            get { return _removable; }
            set { _removable = value; }
        }

        public Type Type { get; internal set; }

        public bool Available
        {
            get
            {
                if (ConnectionLimit == 0)
                    return true;

                return Connections.Count < ConnectionLimit;
            }
        }

        public List<IConnection> Connections
        {
            get
            {
                return Canvas.State
                    .Connections
                    .Where(x => x.FromKnob == this || x.ToKnob == this)
                    .OrderBy(x => (Side == NodeSide.Top || Side == NodeSide.Bottom ?
                    (x.ToKnob == this ? x.FromKnob.CenterForConnection(x).x : x.ToKnob.CenterForConnection(x).x)
                    :
                    (x.ToKnob == this ? x.FromKnob.CenterForConnection(x).y : x.ToKnob.CenterForConnection(x).y)
                    ))
                    .ToList();
            }
        }

        public Vector2 CenterForConnection(IConnection conn)
        {
            if (!Expanded)
                return Rect.center;

            int index = Connections.IndexOf(conn);
            float position = -(Connections.Count * 22) / 2 + 10;
            if (Side == NodeSide.Top || Side == NodeSide.Bottom)
            {
                Vector2 TopCenter = new Vector2(position, Side == NodeSide.Top ? -25 : 25);
                return Rect.center + TopCenter + new Vector2(20 * index + 1, 0);
            }
            else
            {
                Vector2 TopCenter = new Vector2(Side == NodeSide.Left ? -25 : 25, position);
                return Rect.center + TopCenter + new Vector2(0, 20 * index + 1);
            }
        }

        public virtual void DrawKnobWindow()
        {
            string textureName = "";

            switch (Direction)
            {
                case Direction.Input:
                    textureName = Canvas.Theme.InputKnob;
                    break;

                case Direction.Output:
                    textureName = Canvas.Theme.OutputKnob;
                    break;

                case Direction.Both:
                    textureName = Canvas.Theme.BothKnob;
                    break;
            }

            int rotation = 0;

            switch (Side)
            {
                case NodeSide.Bottom:
                    rotation = 1;
                    break;

                case NodeSide.Right:
                    rotation = 2;
                    break;

                case NodeSide.Top:
                    rotation = 3;
                    break;
            }

            Texture2D knobTexture = null;
            Color color = Canvas.Theme.GetColor(Type.Name);

            if (Expanded && Available)
                knobTexture = Canvas.Theme.GetTexture(Canvas.Theme.AddKnobName, 0, color);
            else
                knobTexture = Canvas.Theme.GetTexture(textureName, rotation, color);

            Rect knobRect = Rect;
            knobRect.position += Canvas.State.PanOffset;

            GUI.DrawTexture(knobRect, knobTexture);
        }

        public bool HitTest(Vector2 loc, out IControl hit)
        {
            hit = null;

            if (Rect.Contains(loc))
                hit = this;

            return hit != null;
        }
    }
}