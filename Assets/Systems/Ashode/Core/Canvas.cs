using System;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public class Canvas
    {
        public State State;
        public InputSystem InputSystem = new InputSystem(typeof(InputControls));
        public Theme Theme = new Theme();

        // This is an Action instead of an EventHandler because it
        // makes it quick to subscribe the EditorWindow.Repaint action
        public event Action Repaint;
        internal void OnRepaint() { Repaint.SafeInvoke(); }

        public Canvas(State state) { this.State = state; }

        public virtual void OnGUI() { }

        public void Draw(Rect canvasRect)
        {
            State.CanvasSize = canvasRect;
            InputSystem.HandleEvents(this, false);

            DrawBackground();
            DrawConnections();
            DrawNodes();

            OnGUI();

            InputSystem.HandleEvents(this, true);
        }
        private void DrawBackground()
        {
            if(Event.current.type != EventType.Repaint)
                return;

            float width = Theme.CanvasBackground.width;
            float height = Theme.CanvasBackground.height;
            Vector2 offset = State.PanOffset;

            Rect uvDrawRect = new Rect(-offset.x * width,
                (offset.y - State.CanvasSize.height) * height,
                State.CanvasSize.width * width,
                State.CanvasSize.height * height);

            GUI.DrawTextureWithTexCoords(State.CanvasSize, Theme.CanvasBackground, uvDrawRect);
        }

        private void DrawConnections()
        {
            foreach (var connection in State.Connections)
            {
                connection.DrawConnectionWindow(this);
            }
        }

        private void DrawNodes()
        {
            foreach (var node in State.Nodes)
            {
                node.DrawNodeWindow(this);
            }
        }

        public void AddNode<TNode>(Rect placement)
        {

        }

        // Helpers
        public INode FindNodeAt(Vector2 loc)
        {
            return State.Nodes.FirstOrDefault(x => x.Rect.Contains(loc));
        }

        public IKnob FindKnobAt(Vector2 loc)
        {
            foreach (var node in State.Nodes)
            {
                IKnob knob = node.Knobs.Values.FirstOrDefault(x => x.Rect.Contains(loc));
                if (knob != null)
                    return knob;
            }

            return null;
        }

        public void FindNodeOrKnobAt(Vector2 loc, out INode oNode, out IKnob oKnob)
        {
            oNode = null;
            oKnob = null;

            foreach (var node in State.Nodes)
            {
                if (node.Rect.Contains(loc))
                    oNode = node;

                IKnob knob = node.Knobs.Values.FirstOrDefault(x => x.Rect.Contains(loc));
                if (knob != null)
                    oKnob = knob;
            }
        }

        public Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return (screenPos - State.PanOffset);
        }
    }
}