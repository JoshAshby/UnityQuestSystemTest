using System;
using Ashode.CoreExtensions;

namespace Ashode
{
    public class NodeEventArgs<T> : EventArgs
    {
        public T Target { get; internal set; }
    }

    public class EventSystem
    {
        public event EventHandler<NodeEventArgs<INode>> AddNodeEvent;
        internal void OnAddNode(INode node) { AddNodeEvent.SafeInvoke(this, new NodeEventArgs<INode> { Target = node }); }

        public event EventHandler<NodeEventArgs<INode>> RemoveNodeEvent;
        internal void OnRemoveNode(INode node) { RemoveNodeEvent.SafeInvoke(this, new NodeEventArgs<INode> { Target = node }); }

        // public event EventHandler<NodeEventArgs> MoveNodeOnCanvas;
        // internal void OnMoveNode(INode node) { MoveNodeOnCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        // public event EventHandler<NodeEventArgs> RemoveNodeFromCanvas;
        // internal void OnRemoveNode(INode node) { RemoveNodeFromCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }
    }
}