using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Ashode
{
    public class State
    {
        public Action Repaints;
        public void Repaint() { if (Repaints != null) Repaints(); }

        public List<Node> Nodes;

        public Node SelectedNode = null;
        public Node FocusedNode = null;

        // Draggin, Panning and Zoom
        public bool Panning = false;
        public bool Dragging = false;

        public Vector2 DraggingStart = Vector2.zero;
        public Vector2 DragPosition = Vector2.zero;
        public Vector2 DragOffset = Vector2.zero;

        public Vector2 PanOffset = Vector2.zero;

        // Helpers
        public Node FindNodeAt(Vector2 loc)
        {
            return Nodes.FirstOrDefault(x => x.Rect.Contains(loc));
        }

        public Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return (screenPos - PanOffset);
        }
    }
}