using System;
using Ashode.CoreExtensions;

namespace Ashode
{
    public class TargetEventArgs<T> : EventArgs
    {
        public T Target { get; internal set; }
    }

    public class EventSystem
    {
        public event EventHandler<TargetEventArgs<INode>> AddNode;
        internal void OnAddNode(INode node) { AddNode.SafeInvoke(this, new TargetEventArgs<INode> { Target = node }); }

        public event EventHandler<TargetEventArgs<INode>> MoveNode;
        internal void OnMoveNode(INode node) { MoveNode.SafeInvoke(this, new TargetEventArgs<INode> { Target = node }); }

        public event EventHandler<TargetEventArgs<INode>> RemoveNode;
        internal void OnRemoveNode(INode node) { RemoveNode.SafeInvoke(this, new TargetEventArgs<INode> { Target = node }); }

        public event EventHandler<TargetEventArgs<IConnection>> AddConnection;
        internal void OnAddConnection(IConnection conn) { AddConnection.SafeInvoke(this, new TargetEventArgs<IConnection> { Target = conn }); }
        
        public event EventHandler<TargetEventArgs<IConnection>> RemoveConnection;
        internal void OnRemoveConnection(IConnection conn) { RemoveConnection.SafeInvoke(this, new TargetEventArgs<IConnection> { Target = conn }); }

        public event EventHandler<TargetEventArgs<IKnob>> AddKnob;
        internal void OnAddKnob(IKnob knob) { AddKnob.SafeInvoke(this, new TargetEventArgs<IKnob> { Target = knob }); }
        
        public event EventHandler<TargetEventArgs<IKnob>> RemoveKnob;
        internal void OnRemoveKnob(IKnob knob) { RemoveKnob.SafeInvoke(this, new TargetEventArgs<IKnob> { Target = knob }); }
    }
}