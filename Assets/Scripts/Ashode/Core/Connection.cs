using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface IConnection : IControl
    {
        INodeCanvas Parent { get; }
        INodeCanvas Canvas { get; }
        IState State { get; }

        string ID { get; set; }

        IKnob FromKnob { get; set; }
        INode FromNode { get; }
        IKnob ToKnob { get; set; }
        INode ToNode { get; }

        Type Type { get; }

        void DrawConnectionWindow();
    }

    public class Connection : IConnection
    {
        public static bool Verify(IKnob Knob1, IKnob Knob2)
        {
            if (Knob1 == Knob2)
                return false;

            if (Knob1.Direction == Knob2.Direction)
            {
                if (Knob1.Direction != Direction.Both)
                    return false;

                // Todo: Does this make sense for dual direction knobs?
                if (!Knob1.Type.IsAssignableFrom(Knob2.Type) || !Knob2.Type.IsAssignableFrom(Knob1.Type))
                    return false;
            }

            if (!Knob1.Available || !Knob2.Available)
                return false;

            if(Knob1.Connections.Intersect(Knob2.Connections).Any())
                return false;

            IKnob FromKnob = Knob1.Direction == Direction.Input ? Knob2 : Knob1;
            IKnob ToKnob = FromKnob == Knob2 ? Knob1 : Knob2;

            if (!FromKnob.Type.IsAssignableFrom(ToKnob.Type))
                return false;

            return true;
        }

        public INodeCanvas Parent { get; internal set; }
        public INodeCanvas Canvas { get { return Parent; } }
        public IState State { get { return Canvas.State; } }

        private string _id = Guid.NewGuid().ToString();
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        public IKnob FromKnob { get; set; }
        public INode FromNode { get { return FromKnob.Parent; } }
        public IKnob ToKnob { get; set; }
        public INode ToNode { get { return ToKnob.Parent; } }

        public Type Type { get; }

        public Connection(INodeCanvas canvas, IKnob FromKnob, IKnob ToKnob)
        {
            this.Parent = canvas;

            this.FromKnob = FromKnob;
            this.ToKnob = ToKnob;

            this.Type = FromKnob.Type;
        }

        public virtual void DrawConnectionWindow()
        {
            Vector3 PanOffset = (Vector3)Canvas.State.PanOffset;

            Vector2 startCenter = FromKnob.CenterForConnection(this) + Canvas.State.PanOffset;
            Vector3 startPosition = (Vector3)startCenter;

            Vector2 endCenter = ToKnob.CenterForConnection(this) + Canvas.State.PanOffset;
            Vector3 endPosition = (Vector3)endCenter;

            Vector3 startTangent = startPosition + FromKnob.DirectionVector * 50;
            Vector3 endTangent = endPosition + ToKnob.DirectionVector * 50;

            Handles.DrawBezier(
                startPosition,
                endPosition,
                startTangent,
                endTangent,
                Canvas.Theme.GetColor(Type.Name),
                Canvas.Theme.Line,
                3
            );

            DrawKnob(FromKnob, startCenter);
            DrawKnob(ToKnob, endCenter);
        }

        public void DrawKnob(IKnob knob, Vector2 center)
        {
            if (!knob.Expanded)
                return;

            string textureName = "";
            if (Canvas.State.FocusedConnection == this)
            {
                textureName = Canvas.Theme.RemoveKnobName;
            }
            else
            {
                switch (knob.Direction)
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
            }

            int rotation = 0;
            switch (knob.Side)
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

            knobTexture = Canvas.Theme.GetTexture(textureName, rotation, color);

            Rect knobRect = new Rect(0, 0, 20, 20);
            knobRect.center = center;

            GUI.DrawTexture(knobRect, knobTexture);
        }

        public bool HitTest(Vector2 loc, out IControl hit)
        {
            hit = null;

            Rect rect = new Rect(0, 0, 20, 20);

            rect.center = FromKnob.CenterForConnection(this);
            if (rect.Contains(loc))
                hit = this;

            rect.center = ToKnob.CenterForConnection(this);
            if (rect.Contains(loc))
                hit = this;

            return hit != null;
        }
    }
}