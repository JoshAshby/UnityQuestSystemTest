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
            Texture2D _aaLine = AssetDatabase.LoadAssetAtPath("Assets/Systems/Ashode/Resources/Textures/AA Line@2X.png", typeof(Texture2D)) as Texture2D;
            Vector3 PanOffset = new Vector3(Canvas.State.PanOffset.x, Canvas.State.PanOffset.y, 0);
            Vector3 startPosition = new Vector3(FromKnob.Rect.center.x, FromKnob.Rect.center.y, 0) + PanOffset;
            Vector3 startTangent = startPosition + FromKnob.DirectionVector * 50;

            Vector3 endPosition = new Vector3(ToKnob.Rect.center.x, ToKnob.Rect.center.y, 0) + PanOffset;
            Vector3 endTangent = endPosition + ToKnob.DirectionVector * 50;

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color, _aaLine, 3);
        }

        private Color GetColor()
        {
            int hash = Type.Name.GetHashCode();
            float r = ((hash & 0xFF0000) >> 16) / 100f;
            float g = ((hash & 0x00FF00) >> 8) / 100f;
            float b = (hash & 0x0000FF) / 100f;

            return new Color(r, g, b);
        }
    }
}