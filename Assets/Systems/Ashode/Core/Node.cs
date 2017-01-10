using UnityEditor;
using UnityEngine;
using System;

namespace Ashode
{
    public interface INode
    {
        string ID { get; set; }
        Rect Rect { get; set; }

        string Title { get; set; }

        void OnGUI();
        void DrawNodeWindow(State State);
    }

    public abstract class Node : INode
    {
        public Rect _rect = new Rect(30, 30, 200, 100);
        public Rect Rect
        {
            get { return _rect; }
            set { _rect = value; }
        }

        private string _id = Guid.NewGuid().ToString();
        public string ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _title = "Window";
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        private Vector2 contentOffset;

        public virtual void DrawNodeWindow(State State)
        {
            Rect nodeRect = Rect;

            nodeRect.position += State.PanOffset;
            contentOffset = new Vector2(0, 20);

            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Label(headerRect, Title, State.SelectedNode == this ? EditorStyles.boldLabel : EditorStyles.label);

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y + contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);

            GUI.changed = false;
            OnGUI();

            GUILayout.EndArea();
            GUI.EndGroup();
        }

        public virtual void OnGUI() { }
    }
}