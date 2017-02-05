using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public interface IState
    {
        INodeCanvas Parent { get; }
        INodeCanvas Canvas { get; }

        TNode AddNode<TNode>(Vector2 pos) where TNode : INode;
        INode AddNode(Type type, Vector2 pos);
        void RemoveNode(INode node);
        void RemoveNode(string id);

        IConnection AddConnection(IKnob FromKnob, IKnob ToKnob);
        TConnection AddConnection<TConnection>(IKnob FromKnob, IKnob ToKnob) where TConnection : IConnection;
        IConnection AddConnection(Type type, IKnob FromKnob, IKnob ToKnob);
        void RemoveConnection(IConnection conn);
        void RemoveConnection(string id);

        void Reset();

        List<INode> Nodes { get; }
        List<IConnection> Connections { get; }

        INode FocusedNode { get; set; }
        INode SelectedNode { get; set; }

        IKnob FocusedKnob { get; set; }
        IKnob SelectedKnob { get; set; }
        IKnob ExpandedKnob { get; set; }
        IKnob ConnectedFromKnob { get; set; }

        IConnection FocusedConnection { get; set; }

        bool Panning { get; set; }
        bool Dragging { get; set; }

        Vector2 DraggingStart { get; set; }
        Vector2 DragPosition { get; set; }

        Vector2 DragOffset { get; set; }
        Vector2 PanOffset { get; set; }

        Rect GlobalCanvasSize { get; set; }
        Rect LocalCanvasSize { get; }
    }

    public class State : IState
    {
        public INodeCanvas Parent { get; internal set; }
        public INodeCanvas Canvas { get { return Parent; } }

        // These should probably be properties because they are the exposed API but meh, fuck it
        private List<INode> _nodes = new List<INode>();
        public virtual List<INode> Nodes { get { return _nodes; } }

        private List<IConnection> _connections = new List<IConnection>();
        public virtual List<IConnection> Connections { get { return _connections; } }

        public virtual INode FocusedNode { get; set; }
        public virtual INode SelectedNode { get; set; }

        public virtual IKnob FocusedKnob { get; set; }
        public virtual IKnob SelectedKnob { get; set; }
        public virtual IKnob ExpandedKnob { get; set; }
        public virtual IKnob ConnectedFromKnob { get; set; }

        public virtual IConnection FocusedConnection { get; set; }

        // Draggin, panning, connecting and maybe eventually zoommmmmmz
        public bool Panning { get; set; }
        public bool Dragging { get; set; }

        private Vector2 _draggingStart = Vector2.zero;
        public Vector2 DraggingStart { get { return _draggingStart; } set { _draggingStart = value; } }

        private Vector2 _dragPosition = Vector2.zero;
        public Vector2 DragPosition { get { return _dragPosition; } set { _dragPosition = value; } }

        private Vector2 _dragOffset = Vector2.zero;
        public Vector2 DragOffset { get { return _dragOffset; } set { _dragOffset = value; } }

        private Vector2 _panOffset = Vector2.zero;
        public Vector2 PanOffset { get { return _panOffset; } set { _panOffset = value; } }

        public Rect GlobalCanvasSize { get; set; }
        public Rect LocalCanvasSize
        {
            get
            {
                Rect canvasRect = GlobalCanvasSize;
                canvasRect.position = Vector2.zero;

                return canvasRect;
            }
        }

        public State(NodeCanvas canvas)
        {
            this.Parent = canvas;
        }

        public void Reset()
        {
            FocusedNode = null;
            FocusedKnob = null;
            FocusedConnection = null;

            SelectedNode = null;
            SelectedKnob = null;

            ExpandedKnob = null;
            ConnectedFromKnob = null;

            Panning = false;
            Dragging = false;

            DraggingStart = Vector2.zero;
            DragPosition = Vector2.zero;
            DragOffset = Vector2.zero;
            PanOffset = Vector2.zero;

            _nodes = new List<INode>();
            _connections = new List<IConnection>();
        }

        public TNode AddNode<TNode>(Vector2 pos) where TNode : INode
        {
            return (TNode)AddNode(typeof(TNode), pos);
        }

        public INode AddNode(Type type, Vector2 pos)
        {
            if (!typeof(INode).IsAssignableFrom(type))
                return null;

            if (!Parent.NodeTypes().Any(x => x.NodeType == type))
                return null;

            INode node = (INode)Activator.CreateInstance(type, this.Parent, pos);

            Nodes.Add(node);
            Canvas.EventSystem.OnAddNode(node);

            return node;
        }

        public void RemoveNode(INode node)
        {
            if (!node.Removable)
                return;

            if (FocusedNode == node)
                FocusedNode = null;

            if (SelectedNode == node)
                SelectedNode = null;

            List<IKnob> knobs = node.Knobs.Values.ToList();

            foreach (var knob in knobs)
            {
                if (FocusedKnob == knob)
                    FocusedKnob = null;

                if (SelectedKnob == knob)
                    SelectedNode = null;

                if (ExpandedKnob == knob)
                    ExpandedKnob = null;

                if (ConnectedFromKnob == knob)
                    ConnectedFromKnob = null;

                foreach (var conn in knob.Connections)
                    RemoveConnection(conn);

                node.Knobs.Remove(knob.ID);
            }

            Canvas.EventSystem.OnRemoveNode(node);
            Nodes.Remove(node);
        }

        public void RemoveNode(string id)
        {
            RemoveNode(Nodes.Find(x => x.ID == id));
        }

        public IConnection AddConnection(IKnob FromKnob, IKnob ToKnob)
        {
            IConnection connection = new Connection(this.Parent, FromKnob, ToKnob);

            Connections.Add(connection);
            Canvas.EventSystem.OnAddConnection(connection);

            return connection;
        }

        public TConnection AddConnection<TConnection>(IKnob FromKnob, IKnob ToKnob) where TConnection : IConnection
        {
            return (TConnection)AddConnection(typeof(TConnection), FromKnob, ToKnob);
        }

        public IConnection AddConnection(Type type, IKnob FromKnob, IKnob ToKnob)
        {
            if (!typeof(IConnection).IsAssignableFrom(type))
                return null;

            IConnection connection = (IConnection)Activator.CreateInstance(type, this.Parent, FromKnob, ToKnob);

            Connections.Add(connection);
            Canvas.EventSystem.OnAddConnection(connection);

            return connection;
        }

        public void RemoveConnection(IConnection conn)
        {
            Canvas.EventSystem.OnRemoveConnection(conn);
            Connections.Remove(conn);
        }

        public void RemoveConnection(string id)
        {
            RemoveConnection(Connections.Find(x => x.ID == id));
        }
    }
}