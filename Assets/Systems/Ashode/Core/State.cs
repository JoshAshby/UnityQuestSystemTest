using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ashode
{
    public interface IState
    {
        NodeCanvas Parent { get; }

        INode AddNode(Type type, Rect pos);
        void RemoveNode(INode node);
        void RemoveNode(string id);

        IConnection AddConnection(IKnob FromKnob, IKnob ToKnob);
        IConnection AddConnection(Type type, IKnob FromKnob, IKnob ToKnob);
        void RemoveConnection(IConnection conn);
        void RemoveConnection(string id);

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
        Rect CanvasSize { get; set; }
    }

    public class State : IState
    {
        public NodeCanvas Parent { get; internal set; }

        // These should probably be properties because they are the exposed API but meh, fuck it
        private List<INode> _nodes = new List<INode>();
        public List<INode> Nodes { get { return _nodes; } }

        private List<IConnection> _connections = new List<IConnection>();
        public List<IConnection> Connections { get { return _connections; } }

        public INode FocusedNode { get; set; }

        private INode _selectedNode = null;
        public INode SelectedNode { get { return _selectedNode; } set { _selectedNode = value; } }

        public IKnob FocusedKnob { get; set; }
        public IKnob SelectedKnob { get; set; }
        public IKnob ExpandedKnob { get; set; }
        public IKnob ConnectedFromKnob { get; set; }

        public IConnection FocusedConnection { get; set; }

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

        private Rect _canvasSize = new Rect(0, 0, 800, 600);
        public Rect CanvasSize { get { return _canvasSize; } set { _canvasSize = value; } }

        public State(NodeCanvas canvas)
        {
            this.Parent = canvas;
        }

        public INode AddNode(Type type, Rect pos)
        {
            if (!typeof(INode).IsAssignableFrom(type))
                return null;

            INode node = (INode)Activator.CreateInstance(type, this.Parent, pos);

            Nodes.Add(node);

            return node;
        }

        public void RemoveNode(INode node)
        {
            List<IKnob> knobs = node.Knobs.Values.ToList();

            foreach (var knob in knobs)
                node.RemoveKnob(knob.ID);

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

            return connection;
        }

        public IConnection AddConnection(Type type, IKnob FromKnob, IKnob ToKnob)
        {
            if (!typeof(IConnection).IsAssignableFrom(type))
                return null;

            IConnection connection = (IConnection)Activator.CreateInstance(type, this.Parent, FromKnob, ToKnob);

            Connections.Add(connection);

            return connection;
        }

        public void RemoveConnection(IConnection conn)
        {
            Connections.Remove(conn);
        }

        public void RemoveConnection(string id)
        {
            RemoveConnection(Connections.Find(x => x.ID == id));
        }
    }
}