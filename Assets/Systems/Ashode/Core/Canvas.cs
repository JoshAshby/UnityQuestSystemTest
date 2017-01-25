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
        State State { get; set; }
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
        public State State { get; set; }

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
            State.CanvasSize = canvasRect;
            InputSystem.HandleEvents(this, false);

            DrawBackground();

            DrawConnections();
            DrawNodes();

            DrawCurveToMouse();

            OnGUI();

            InputSystem.HandleEvents(this, true);
        }

        public virtual void DrawBackground()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            float width = 1f / Theme.CanvasBackground.width;
            float height = 1f / Theme.CanvasBackground.height;
            Vector2 offset = State.PanOffset;

            Rect uvDrawRect = new Rect(0, 0, State.CanvasSize.width * width, State.CanvasSize.height * height);

            GUI.DrawTextureWithTexCoords(State.CanvasSize, Theme.CanvasBackground, uvDrawRect);
        }

        private void DrawCurveToMouse()
        {
            if (!State.Connecting)
                return;

            if (State.SelectedKnob == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            Vector3 PanOffset = (Vector3)State.PanOffset;

            Vector3 startPosition = (Vector3)State.SelectedKnob.Rect.center + PanOffset;
            Vector3 startTangent = startPosition + State.SelectedKnob.DirectionVector * 50;

            Vector2 guiMouse = ScreenToCanvasSpace(Event.current.mousePosition);
            Vector3 endPosition = (Vector3)guiMouse + PanOffset;
            Vector3 endTangent = endPosition + Vector3.up * 50;

            Color color = Color.black;
            if (State.FocusedKnob != null)
                if (!Connection.Verify(State.SelectedKnob, State.FocusedKnob))
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

            foreach (var node in State.Nodes)
                if (node.HitTest(loc, out hit))
                    return hit;

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