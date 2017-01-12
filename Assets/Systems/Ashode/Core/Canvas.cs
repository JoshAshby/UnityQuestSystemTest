using System;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public static class EventExtensions
    {
        public static void SafeInvoke<TEventArgs>(this EventHandler<TEventArgs> handler, object sender, TEventArgs args) where TEventArgs : EventArgs
        {
            var e = handler;
            if (e != null)
                e(sender, args);
        }

        public static void SafeInvoke(this Action handler)
        {
            var e = handler;
            if (e != null)
                e();
        }
    }

    public class NodeEventArgs : EventArgs
    {
        public INode Node { get; internal set; }
    }

    public class Canvas
    {
        public State State;
        public InputSystem InputSystem = new InputSystem(typeof(InputControls));

        // This is an Action instead of an EventHandler because it
        // makes it quick to subscribe the EditorWindow.Repaint action
        public event Action Repaint;
        internal void OnRepaint() { Repaint.SafeInvoke(); }

        public event EventHandler<NodeEventArgs> AddNodeToCanvas;
        internal void OnAddNode(Node node) { AddNodeToCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        public event EventHandler<NodeEventArgs> MoveNodeOnCanvas;
        internal void OnMoveNode(Node node) { MoveNodeOnCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        public event EventHandler<NodeEventArgs> RemoveNodeFromCanvas;
        internal void OnRemoveNode(Node node) { RemoveNodeFromCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        public Canvas(State state) { this.State = state; }

        public virtual void OnGUI() { }

        public void Draw(Rect canvasRect)
        {
            State.CanvasSize = canvasRect;

            InputSystem.HandleEvents(this, false);

            DrawConnections();
            DrawNodes();

            OnGUI();

            InputSystem.HandleEvents(this, true);
        }

        private void DrawConnections() { }

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
        public Node FindNodeAt(Vector2 loc)
        {
            return State.Nodes.FirstOrDefault(x => x.Rect.Contains(loc));
        }

        public Knob FindKnobAt(Vector2 loc)
        {
            foreach (var node in State.Nodes)
            {
                Knob knob = node.Knobs.Values.FirstOrDefault(x => x.Rect.Contains(loc));
                if (knob != null)
                    return knob;
            }

            return null;
        }

        public void FindNodeOrKnobAt(Vector2 loc, out Node oNode, out Knob oKnob)
        {
            oNode = null;
            oKnob = null;

            foreach (var node in State.Nodes)
            {
                if(node.Rect.Contains(loc))
                    oNode = node;

                Knob knob = node.Knobs.Values.FirstOrDefault(x => x.Rect.Contains(loc));
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