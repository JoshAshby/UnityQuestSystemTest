using System;
using System.Collections.Generic;
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
        string ID { get; set; }

        Rect Rect { get; set; }
        float Offset { get; set; }
        NodeSide Side { get; set; }
        Vector3 DirectionVector { get; }

        Direction Direction { get; set; }
        bool AllowMultiple { get; set; }

        void DrawKnobWindow(Canvas Canvas);

        Type Type { get; }
    }

    public class Knob<TAccept> : IKnob
    {
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

        private float _offset = 2;
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
            get {
                switch(Side)
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

        public Type Type { get { return typeof(TAccept); } }

        public virtual void DrawKnobWindow(Canvas Canvas)
        {
            string text = "";

            switch (Direction)
            {
                case Direction.Input:
                    text = "I";
                    break;

                case Direction.Output:
                    text = "O";
                    break;

                case Direction.Both:
                    text = "B";
                    break;
            }

            Rect knobRect = Rect;
            knobRect.position += Canvas.State.PanOffset;

            GUI.Box(knobRect, text, GUI.skin.box);
        }
    }
}