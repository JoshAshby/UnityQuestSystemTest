using System;
using UnityEditor;
using UnityEngine;
using Ashode.CoreExtensions;
using System.Collections.Generic;
using System.Linq;

namespace Ashode
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NodeBelongsToAttribute : Attribute
    {
        public Type CanvasType { get; set; }
        public string Name { get; set; }
        public bool Hidden { get; set; }

        public NodeBelongsToAttribute(Type canvasType)
        {
            this.CanvasType = canvasType;
        }
    }

    public class NodeBelongsToAttributeInfo
    {
        public Type CanvasType { get; }
        public string Name { get; }
        public bool Hidden { get; }
        public Type NodeType { get; }

        public NodeBelongsToAttributeInfo(Type x, NodeBelongsToAttribute y)
        {
            this.NodeType = x;
            this.CanvasType = y.CanvasType;
            this.Name = y.Name;
            this.Hidden = y.Hidden;
        }
    }

    public interface IControl
    {
        bool HitTest(Vector2 loc, out IControl hit);
    }

    public interface INodeCanvas
    {
        IState State { get; set; }
        InputSystem InputSystem { get; set; }
        ITheme Theme { get; set; }

        // This is an Action instead of an EventHandler because it
        // makes it quick to subscribe the EditorWindow.Repaint action
        event Action Repaint;
        void OnRepaint();

        void OnGUI();

        void Draw(Rect canvasRect);
        void DrawBackground();
        void DrawCurveToMouse();

        IControl FindControlAt(Vector2 loc);
        Vector2 ScreenToCanvasSpace(Vector2 screenPos);
        List<NodeBelongsToAttributeInfo> NodeTypes();
    }

    public class NodeCanvas : INodeCanvas
    {
        public IState State { get; set; }
        public InputSystem InputSystem { get; set; }
        public ITheme Theme { get; set; }

        public event Action Repaint;
        public void OnRepaint() { Repaint.SafeInvoke(); }

        public NodeCanvas()
        {
            State = new State(this);
            InputSystem = new InputSystem(typeof(InputControls));
            Theme = new Theme();
        }

        public virtual void OnGUI() { }

        public virtual void Draw(Rect canvasRect)
        {
            State.GlobalCanvasSize = canvasRect;

            GUI.BeginGroup(canvasRect, GUI.skin.box);
            InputSystem.HandleEvents(this, false);

            DrawBackground();

            DrawConnections();
            DrawNodes();

            DrawCurveToMouse();

            OnGUI();

            InputSystem.HandleEvents(this, true);
            GUI.EndGroup();
        }

        public virtual void DrawBackground()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            float width = 1f / Theme.CanvasBackground.width;
            float height = 1f / Theme.CanvasBackground.height;
            Vector2 offset = State.PanOffset;

            Rect uvDrawRect = new Rect(-offset.x * width, (offset.y - State.LocalCanvasSize.height) * height, State.LocalCanvasSize.width * width, State.LocalCanvasSize.height * height);

            GUI.DrawTextureWithTexCoords(State.LocalCanvasSize, Theme.CanvasBackground, uvDrawRect);
        }

        public virtual void DrawCurveToMouse()
        {
            if (State.ConnectedFromKnob == null)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            Vector3 PanOffset = (Vector3)State.PanOffset;

            Vector3 startPosition = (Vector3)State.ConnectedFromKnob.Rect.center + PanOffset;
            Vector3 startTangent = startPosition + State.ConnectedFromKnob.DirectionVector * 50;

            Vector2 guiMouse = ScreenToCanvasSpace(Event.current.mousePosition);
            Vector3 endPosition = (Vector3)guiMouse + PanOffset;
            Vector3 endTangent = endPosition + Vector3.up * 50;

            Color color = Color.black;
            if (State.FocusedKnob != null)
                if (!Connection.Verify(State.ConnectedFromKnob, State.FocusedKnob))
                    color = Color.red;
                else
                    color = Color.green;

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, color, Theme.Line, 3);
            OnRepaint();
        }

        protected void DrawConnections()
        {
            foreach (var connection in State.Connections)
                connection.DrawConnectionWindow();
        }

        protected void DrawNodes()
        {
            // Make sure the top node is the selected one
            if (Event.current.type == EventType.Layout && State.SelectedNode != null)
            {
                State.Nodes.Remove(State.SelectedNode);
                State.Nodes.Add(State.SelectedNode);
            }

            foreach (var node in State.Nodes)
                node.DrawNodeWindow();
        }

        // Helpers
        public IControl FindControlAt(Vector2 loc)
        {
            IControl hit = null;

            for (int i = State.Nodes.Count - 1; i >= 0; i--)
            {
                var node = State.Nodes[i];
                if (node.HitTest(loc, out hit))
                    return hit;
            }

            foreach (var conn in State.Connections)
                if (conn.HitTest(loc, out hit))
                    return hit;

            return hit;
        }

        public Vector2 ScreenToCanvasSpace(Vector2 screenPos)
        {
            return (screenPos - State.PanOffset);
        }

        protected List<NodeBelongsToAttributeInfo> _nodeInfo = null;
        public List<NodeBelongsToAttributeInfo> NodeTypes()
        {
            if (_nodeInfo != null)
                return _nodeInfo;

            return _nodeInfo = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces().Contains(typeof(Ashode.INode)))
                .SelectMany(x =>
                {
                    return x.GetCustomAttributes(typeof(NodeBelongsToAttribute), false)
                        .Select(y => (NodeBelongsToAttribute)y)
                        .Where(y => y.CanvasType == this.GetType())
                        .Select(y => new NodeBelongsToAttributeInfo(x, y));
                }).ToList();
        }
    }
}