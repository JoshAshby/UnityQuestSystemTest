using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public interface IControl
    {
        bool HitTest(Vector2 loc, out IControl hit);
    }

    public interface INodeCanvas
    {
        IState State { get; set; }
        InputSystem InputSystem { get; set; }
        Theme Theme { get; set; }

        // This is an Action instead of an EventHandler because it
        // makes it quick to subscribe the EditorWindow.Repaint action
        event Action Repaint;
        void OnRepaint();

        void OnGUI();

        void Draw(Rect canvasRect);
        void DrawBackground();

        IControl FindControlAt(Vector2 loc);
        Vector2 ScreenToCanvasSpace(Vector2 screenPos);
    }

    public class NodeCanvas : INodeCanvas
    {
        public IState State { get; set; }

        private InputSystem _inputSystem = new InputSystem(typeof(InputControls));
        public InputSystem InputSystem
        {
            get { return _inputSystem; }
            set { _inputSystem = value; }

        }

        private Theme _theme = new Theme();
        public Theme Theme
        {
            get { return _theme; }
            set { _theme = value; }
        }

        public event Action Repaint;
        public void OnRepaint() { Repaint.SafeInvoke(); }

        public virtual void OnGUI() { }

        public virtual void Draw(Rect canvasRect)
        {
            State.GlobalCanvasSize = canvasRect;

            GUI.BeginGroup(canvasRect, GUI.skin.box);
            InputSystem.HandleEvents(this, false);

            DrawBackground();

            DrawConnections();
            DrawNodes();

            DrawCurveToMouse();

            OnGUI();

            InputSystem.HandleEvents(this, true);
            GUI.EndGroup();
        }

        public virtual void DrawBackground()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            float width = 1f / Theme.CanvasBackground.width;
            float height = 1f / Theme.CanvasBackground.height;
            Vector2 offset = State.PanOffset;

            Rect uvDrawRect = new Rect(-offset.x * width, (offset.y - State.LocalCanvasSize.height) * height, State.LocalCanvasSize.width * width, State.LocalCanvasSize.height * height);

            GUI.DrawTextureWithTexCoords(State.LocalCanvasSize, Theme.CanvasBackground, uvDrawRect);
        }

        private void DrawCurveToMouse()
        {
            if (State.ConnectedFromKnob == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            Vector3 PanOffset = (Vector3)State.PanOffset;

            Vector3 startPosition = (Vector3)State.ConnectedFromKnob.Rect.center + PanOffset;
            Vector3 startTangent = startPosition + State.ConnectedFromKnob.DirectionVector * 50;

            Vector2 guiMouse = ScreenToCanvasSpace(Event.current.mousePosition);
            Vector3 endPosition = (Vector3)guiMouse + PanOffset;
            Vector3 endTangent = endPosition + Vector3.up * 50;

            Color color = Color.black;
            if (State.FocusedKnob != null)
                if (!Connection.Verify(State.ConnectedFromKnob, State.FocusedKnob))
                    color = Color.red;
                else
                    color = Color.green;

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, Theme.Line, 3);
            OnRepaint();
        }

        private void DrawConnections()
        {
            foreach (var connection in State.Connections)
                connection.DrawConnectionWindow();
        }

        private void DrawNodes()
        {
            // Make sure the top node is the selected one
            if (Event.current.type == EventType.Layout && State.SelectedNode != null)
            {
                State.Nodes.Remove(State.SelectedNode);
                State.Nodes.Add(State.SelectedNode);
            }

            foreach (var node in State.Nodes)
                node.DrawNodeWindow();
        }

        // Helpers
        public IControl FindControlAt(Vector2 loc)
        {
            IControl hit = null;

            for(int i = State.Nodes.Count-1; i >= 0; i--)
            {
                var node = State.Nodes[i];
                if (node.HitTest(loc, out hit))
                    return hit;
            }

            foreach (var conn in State.Connections)
                if (conn.HitTest(loc, out hit))
                    return hit;

            return hit;
        }

        public Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return (screenPos - State.PanOffset);
        }
    }
}