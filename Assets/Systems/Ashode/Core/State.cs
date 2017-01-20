using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ashode
{
    // public class NodeEventArgs : EventArgs
    // {
    //     public INode Node { get; internal set; }
    // }

    public class State
    {
        public Canvas Parent { get; internal set; }

        // These should probably be properties because they are the exposed API but meh, fuck it
        public List<INode> Nodes = new List<INode>();
        public List<IConnection> Connections = new List<IConnection>();

        public INode SelectedNode = null;
        public INode FocusedNode = null;

        public IKnob SelectedKnob = null;
        public IKnob FocusedKnob = null;

        // Draggin, Panning and Zoom
        public bool Panning = false;
        public bool Dragging = false;

        public Vector2 DraggingStart = Vector2.zero;
        public Vector2 DragPosition = Vector2.zero;

        public Vector2 DragOffset = Vector2.zero;
        public Vector2 PanOffset = Vector2.zero;
        public Rect CanvasSize = new Rect(0, 0, 800, 600);

        // public event EventHandler<NodeEventArgs> AddNodeToCanvas;
        // internal void OnAddNode(INode node) { AddNodeToCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        // public event EventHandler<NodeEventArgs> MoveNodeOnCanvas;
        // internal void OnMoveNode(INode node) { MoveNodeOnCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        // public event EventHandler<NodeEventArgs> RemoveNodeFromCanvas;
        // internal void OnRemoveNode(INode node) { RemoveNodeFromCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        public State(Canvas canvas)
        {
            this.Parent = canvas;
        }

        public TNode AddNode<TNode>(Rect pos) where TNode : Node, INode, new()
        {
            TNode node = new TNode(this, pos);

            Nodes.Add(node);

            return node;
        }
    }
}