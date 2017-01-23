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

    public interface IKnob
    {
        INode Parent { get; }
        ICanvas Canvas { get; }

        string ID { get; set; }

        Rect Rect { get; set; }
        float Offset { get; set; }

        NodeSide Side { get; set; }
        Vector3 DirectionVector { get; }

        Direction Direction { get; set; }
        bool AllowMultiple { get; set; }
        bool Removable { get; set; }

        Type Type { get; }

        List<IConnection> Connections { get; }

        bool Available();
        Vector2 CenterForConnection(IConnection conn);

        void DrawKnobWindow();
    }

    public class Knob : IKnob
    {
        public INode Parent { get; internal set; }
        public ICanvas Canvas { get { return Parent.Parent.Parent; } }

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

        private bool _allowMultiple = false;
        public bool AllowMultiple
        {
            get { return _allowMultiple; }
            set { _allowMultiple = value; }
        }

        private bool _removable = true;
        public bool Removable
        {
            get { return _removable; }
            set { _removable = value; }
        }

        public Type Type { get; internal set; }

        public List<IConnection> Connections
        {
            get { return Canvas.State.Connections.Where(x => x.FromKnob == this || x.ToKnob == this).ToList(); }
        }

        public bool Available()
        {
            bool hasConns = Canvas.State.Connections
                .Where(x => x.FromKnob == this || x.ToKnob == this)
                .Any();

            if (hasConns && !AllowMultiple)
                return false;

            return true;
        }

        public Vector2 CenterForConnection(IConnection conn)
        {
            if(Canvas.State.SelectedKnob != this)
                return Rect.center;

            int index = Connections.IndexOf(conn);
            Vector2 TopCenter = new Vector2(25, -((Connections.Count*20+2))/2+10);
            return Rect.center + TopCenter + new Vector2(0, 20*index+1);
        }

        public virtual void DrawKnobWindow()
        {
            if(Canvas.State.FocusedKnob == this)
                Offset = 5;
            else
                Offset = 0;

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

            Texture2D knobTexture = Canvas.Theme.GetTexture(textureName, rotation, Canvas.Theme.GetColor(Type.Name));

            Rect knobRect = Rect;
            knobRect.position += Canvas.State.PanOffset;

            GUI.DrawTexture(knobRect, knobTexture);

            if (Canvas.State.SelectedKnob != this)
                return;

            Vector2 TopCenter = new Vector2(25, -((((Connections.Count*22)+4))/2)+11);

            for (int i = 0; i < Connections.Count; i++)
            {
                Rect rect = new Rect(0, 0, 20, 20);
                rect.center = knobRect.center + TopCenter + new Vector2(0, (22*i));
                GUI.DrawTexture(rect, knobTexture);
            }
        }
    }
}