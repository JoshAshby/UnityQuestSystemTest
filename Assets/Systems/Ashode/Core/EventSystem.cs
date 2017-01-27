using System;
using Ashode.CoreExtensions;

namespace Ashode
{
    // public class NodeEventArgs : EventArgs
    // {
    //     public INode Node { get; internal set; }
    // }

    public class EventSystem
    {
        public event Action Repaint;
        public void OnRepaint() { Repaint.SafeInvoke(); }

        // public event EventHandler<NodeEventArgs> AddNodeToCanvas;
        // internal void OnAddNode(INode node) { AddNodeToCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        // public event EventHandler<NodeEventArgs> MoveNodeOnCanvas;
        // internal void OnMoveNode(INode node) { MoveNodeOnCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }

        // public event EventHandler<NodeEventArgs> RemoveNodeFromCanvas;
        // internal void OnRemoveNode(INode node) { RemoveNodeFromCanvas.SafeInvoke(this, new NodeEventArgs { Node = node }); }
    }
}