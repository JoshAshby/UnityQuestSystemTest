using System;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface IConnection
    {
        INode FromNode { get; set; }
        IKnob FromKnob { get; set; }

        INode ToNode { get; set; }
        IKnob ToKnob { get; set; }

        Type Type { get; }

        Color Color { get; }

        void DrawConnectionWindow(Canvas Canvas);
    }

    public class Connection<TType> : IConnection
    {
        public INode FromNode { get; set; }
        public IKnob FromKnob { get; set; }

        public INode ToNode { get; set; }
        public IKnob ToKnob { get; set; }

        public Type Type { get { return typeof(TType); } }

        public Color Color { get { return GetColor(); } }

        public void DrawConnectionWindow(Canvas Canvas)
        {
            Vector3 PanOffset = new Vector3(Canvas.State.PanOffset.x, Canvas.State.PanOffset.y, 0);
            Vector3 startPosition = new Vector3(FromKnob.Rect.center.x, FromKnob.Rect.center.y, 0) + PanOffset;
            Vector3 startTangent = startPosition + FromKnob.DirectionVector * 50;

            Vector3 endPosition = new Vector3(ToKnob.Rect.center.x, ToKnob.Rect.center.y, 0) + PanOffset;
            Vector3 endTangent = endPosition + ToKnob.DirectionVector * 50;

            Color shadowColor = Color;
            shadowColor.a = 0.10f;

            // Debug.LogFormat("[{0}] - connection: [{1}, {2}] | event: {3} | start: {4} | end: {5}", DateTime.UtcNow.Ticks, FromKnob.ID, ToKnob.ID, Event.current.type.ToString(), startPosition.ToString(), endPosition.ToString());

            for (int i = 0; i < 3; i++)
                Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, shadowColor, null, (i + 1) * 5);

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color, null, 1);
        }

        private Color GetColor()
        {
            int hash = Type.Name.GetHashCode();
            int r = (hash & 0xFF0000) >> 16;
            int g = (hash & 0x00FF00) >> 8;
            int b = hash & 0x0000FF;

            return new Color(r, g, b);
        }
    }
}