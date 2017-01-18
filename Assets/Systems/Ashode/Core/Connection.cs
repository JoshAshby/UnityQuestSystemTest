using System;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface IConnection
    {
        IKnob FromKnob { get; set; }
        IKnob ToKnob { get; set; }

        Type Type { get; }

        void DrawConnectionWindow(Canvas Canvas);
    }

    public class Connection : IConnection
    {
        public IKnob FromKnob { get; set; }
        public IKnob ToKnob { get; set; }

        public Type Type { get; }

        public Connection(IKnob FromKnob, IKnob ToKnob)
        {

        }

        public void DrawConnectionWindow(Canvas Canvas)
        {
            Vector3 PanOffset = new Vector3(Canvas.State.PanOffset.x, Canvas.State.PanOffset.y, 0);

            Vector3 startPosition = new Vector3(FromKnob.Rect.center.x, FromKnob.Rect.center.y, 0) + PanOffset;
            Vector3 startTangent = startPosition + FromKnob.DirectionVector * 50;

            Vector3 endPosition = new Vector3(ToKnob.Rect.center.x, ToKnob.Rect.center.y, 0) + PanOffset;
            Vector3 endTangent = endPosition + ToKnob.DirectionVector * 50;

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Canvas.Theme.GetColor(Type.Name), Canvas.Theme.Line, 3);
        }
    }
}