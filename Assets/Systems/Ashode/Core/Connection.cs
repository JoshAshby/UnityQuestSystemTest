using System;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface IConnection
    {
        ICanvas Parent { get; }
        ICanvas Canvas { get; }

        IKnob FromKnob { get; set; }
        INode FromNode { get; }
        IKnob ToKnob { get; set; }
        INode ToNode { get; }

        Type Type { get; }

        void DrawConnectionWindow();
    }

    public class Connection : IConnection
    {
        public ICanvas Parent { get; internal set; }
        public ICanvas Canvas { get { return Parent; } }

        public IKnob FromKnob { get; set; }
        public INode FromNode { get { return FromKnob.Parent; } }
        public IKnob ToKnob { get; set; }
        public INode ToNode { get { return ToKnob.Parent; } }

        public Type Type { get; }

        public Connection(Canvas canvas, IKnob FromKnob, IKnob ToKnob)
        {
            this.Parent = canvas;

            this.FromKnob = FromKnob;
            this.ToKnob = ToKnob;

            this.Type = FromKnob.Type;
        }

        public void DrawConnectionWindow()
        {
            Vector3 PanOffset = (Vector3)Canvas.State.PanOffset;

            Vector3 startPosition = (Vector3)FromKnob.CenterForConnection(this) + PanOffset;
            Vector3 endPosition = (Vector3)ToKnob.CenterForConnection(this) + PanOffset;

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
        }

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

            if (!Knob1.Available() || !Knob2.Available())
                return false;

            IKnob FromKnob = Knob1.Direction == Direction.Input ? Knob2 : Knob1;
            IKnob ToKnob = FromKnob == Knob2 ? Knob1 : Knob2;

            if (!FromKnob.Type.IsAssignableFrom(ToKnob.Type))
                return false;

            return true;
        }
    }
}