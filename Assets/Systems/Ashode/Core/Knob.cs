using System;
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

        NodeSide Side { get; set; }
        Direction Direction { get; set; }
        bool AllowMultiple { get; set; }

        void DrawKnobWindow(Canvas Canvas);

        // Type Type { get; set; }
        // object Value { get; set; }

        TResult GetValue<TResult>();
    }

    public class Knob : IKnob
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

        private bool _allowMultiple = false;
        public bool AllowMultiple
        {
            get { return _allowMultiple; }
            set { _allowMultiple = value; }
        }

        public virtual void DrawKnobWindow(Canvas Canvas)
        {
            GUI.Box(Rect, "", GUI.skin.box);
        }

        public Type Type { get; }
        public object Value { get; internal set; }

        public TResult GetValue<TResult>()
        {
            return (TResult)Value;
        }
    }
}