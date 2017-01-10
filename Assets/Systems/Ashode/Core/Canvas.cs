using System;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public class Canvas
    {
        public State State;
        public InputSystem InputSystem;
        
        public Action Repaint;
        public void OnRepaint() { if (Repaint != null) Repaint(); }

        public virtual void OnGUI() { }

        public void Draw(Rect canvasRect)
        {
            InputSystem.HandleEvents(this, false);

            DrawConnections();
            DrawNodes();

            OnGUI();

            InputSystem.HandleEvents(this, true);
        }

        private void DrawConnections() { }

        private void DrawNodes()
        {
            for (int i = 0; i < State.Nodes.Count; i++)
            {
                State.Nodes[i].DrawNodeWindow(this);
            }
        }

        // Helpers
        public Node FindNodeAt(Vector2 loc)
        {
            return State.Nodes.FirstOrDefault(x => x.Rect.Contains(loc));
        }

        public Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return (screenPos - State.PanOffset);
        }
    }
}