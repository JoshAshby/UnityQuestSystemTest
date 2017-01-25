using System;
using System.Collections.Generic;
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
    }

    public class State : IState
    {
        public NodeCanvas Parent { get; internal set; }

        // These should probably be properties because they are the exposed API but meh, fuck it
        public List<INode> Nodes = new List<INode>();
        public List<IConnection> Connections = new List<IConnection>();

        public INode FocusedNode = null;
        public INode SelectedNode = null;

        public IKnob FocusedKnob = null;
        public IKnob SelectedKnob = null;
        public IKnob ExpandedKnob = null;

        // Draggin, panning, connecting and maybe eventually zoommmmmmz
        public bool Panning = false;
        public bool Dragging = false;
        public bool Connecting = false;

        public Vector2 DraggingStart = Vector2.zero;
        public Vector2 DragPosition = Vector2.zero;

        public Vector2 DragOffset = Vector2.zero;
        public Vector2 PanOffset = Vector2.zero;
        public Rect CanvasSize = new Rect(0, 0, 800, 600);

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
            foreach (var knob in node.Knobs)
                node.RemoveKnob(knob.Key);

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