using System.Collections.Generic;
using UnityEngine;

namespace Ashode
{
    public class State
    {
        public List<Node> Nodes = new List<Node>();
        // public List<Connections> Connections;

        public Node SelectedNode = null;
        public Node FocusedNode = null;

        public Knob SelectedKnob = null;
        public Knob FocusedKnob = null;

        // Draggin, Panning and Zoom
        public bool Panning = false;
        public bool Dragging = false;

        public Vector2 DraggingStart = Vector2.zero;
        public Vector2 DragPosition = Vector2.zero;

        public Vector2 DragOffset = Vector2.zero;
        public Vector2 PanOffset = Vector2.zero;
        public Rect CanvasSize;
    }
}