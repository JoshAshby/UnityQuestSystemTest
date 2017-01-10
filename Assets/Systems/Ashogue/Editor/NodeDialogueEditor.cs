using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;
using Ashogue;
using Ashogue.Data;
using Ashogue.Extensions;
using System.Reflection;

namespace Ashode
{
    public class InputEvent
    {
        public Event Event;
        public State State;
    }

    class EventAttributeInfo
    {
        public EventType? EventType { get; set; }
        public int Priority { get; set; }

        public Action<InputEvent> Action { get; set; }

        public EventAttributeInfo(EventHandlerAttribute attribute, MethodInfo method)
        {
            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
        }
    }

    public class InputSystem
    {
        private List<EventAttributeInfo> EventAttributeInfos;

        public InputSystem(Type controlContainer)
        {
            EventAttributeInfos = new List<EventAttributeInfo>();

            MethodInfo[] methods = controlContainer.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (var method in methods)
            {
                EventHandlerAttribute[] attributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), true) as EventHandlerAttribute[];
                if (attributes.Length <= 0)
                    continue;

                foreach (var attribute in attributes)
                {
                    EventAttributeInfos.Add(new EventAttributeInfo(attribute, method));
                }
            }

            EventAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void HandleEvents(State state, bool late)
        {
            InputEvent inputEvent = new InputEvent { Event = Event.current, State = state };

            Func<EventAttributeInfo, bool> predicate = x => (late ? x.Priority >= 100 : x.Priority < 100) && x.EventType == null || x.EventType == inputEvent.Event.type;
            foreach (var info in EventAttributeInfos.Where(predicate))
            {
                info.Action(inputEvent);

                if (inputEvent.Event.type == EventType.Used)
                    return;
            }
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public EventType? EventType { get; set; }
        public int Priority { get; set; }

        public EventHandlerAttribute()
        {
            this.EventType = null;
            this.Priority = 50;
        }

        public EventHandlerAttribute(int priority)
        {
            this.EventType = null;
            this.Priority = priority;
        }

        public EventHandlerAttribute(EventType type)
        {
            this.EventType = type;
            this.Priority = 50;
        }

        public EventHandlerAttribute(EventType type, int priority)
        {
            this.EventType = type;
            this.Priority = priority;
        }
    }

    public static class InputControls
    {
        // Hover on node
        [EventHandler(-4)]
        public static void HandleNodeFocus(InputEvent inputEvent)
        {
            Vector2 canvasSpace = inputEvent.State.ScreenToCanvasSpace(inputEvent.Event.mousePosition);
            inputEvent.State.FocusedNode = inputEvent.State.FindNodeAt(canvasSpace);
        }

        // Click on node
        [EventHandler(EventType.MouseDown, -2)]
        public static void HandleNodeClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.Event.button != 0)
                return;

            if (inputEvent.State.FocusedNode == inputEvent.State.SelectedNode)
                return;

            Vector2 canvasSpace = inputEvent.State.ScreenToCanvasSpace(inputEvent.Event.mousePosition);
            inputEvent.State.SelectedNode = inputEvent.State.FindNodeAt(canvasSpace);
            inputEvent.State.Repaint();
        }

        // Drag node
        [EventHandler(EventType.MouseDown, 110)]
        public static void HandleNodeDragStart(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.Event.button != 0)
                return;

            if (inputEvent.State.FocusedNode == null)
                return;

            if (inputEvent.State.FocusedNode != inputEvent.State.SelectedNode)
                return;

            inputEvent.State.Dragging = true;
            inputEvent.State.DraggingStart = inputEvent.Event.mousePosition;
            inputEvent.State.DragPosition = inputEvent.State.SelectedNode.Rect.position;
            inputEvent.State.DragOffset = Vector2.zero;
            inputEvent.Event.delta = Vector2.zero;
        }

        [EventHandlerAttribute(EventType.MouseDrag)]
        public static void HandleNodeDragging(InputEvent inputEvent)
        {
            if (!inputEvent.State.Dragging)
                return;

            if (inputEvent.State.SelectedNode == null || GUIUtility.hotControl != 0)
            {
                inputEvent.State.Dragging = false;
                return;
            }

            inputEvent.State.DragOffset = inputEvent.Event.mousePosition - inputEvent.State.DraggingStart;
            inputEvent.State.SelectedNode.Rect.position = inputEvent.State.DragPosition + inputEvent.State.DragOffset * inputEvent.State.Zoom;
            inputEvent.State.Repaint();
        }

        [EventHandlerAttribute(EventType.MouseDown)]
        [EventHandlerAttribute(EventType.MouseUp)]
        public static void HandleNodeDragStop(InputEvent inputEvent)
        {
            inputEvent.State.Dragging = false;
        }

        // Panning
        [EventHandlerAttribute(EventType.MouseDown, 100)]
        public static void HandlePanStart(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.Event.button != 0)
                return;

            if (inputEvent.State.FocusedNode != null)
                return;

            inputEvent.State.Panning = true;
            inputEvent.State.DraggingStart = inputEvent.Event.mousePosition;
            inputEvent.State.DragOffset = Vector2.zero;
        }

        [EventHandlerAttribute(EventType.MouseDrag)]
        public static void HandlePanDragging(InputEvent inputEvent)
        {
            if (!inputEvent.State.Panning)
                return;

            if (inputEvent.State.FocusedNode != null || GUIUtility.hotControl != 0)
            {
                inputEvent.State.Panning = false;
                return;
            }

            Vector2 panOffsetChange = inputEvent.State.DragOffset;
            inputEvent.State.DragOffset = inputEvent.Event.mousePosition - inputEvent.State.DraggingStart;
            panOffsetChange = (inputEvent.State.DragOffset - panOffsetChange) * inputEvent.State.Zoom;
            inputEvent.State.PanOffset += panOffsetChange;
            inputEvent.State.Repaint();
        }

        [EventHandlerAttribute(EventType.MouseDown)]
        [EventHandlerAttribute(EventType.MouseUp)]
        public static void HandlePanStop(InputEvent inputEvent)
        {
            inputEvent.State.Panning = false;
        }
    }

    public class State
    {
        public Action Repaints;
        public void Repaint() { if(Repaints != null) Repaints(); }

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
        public float Zoom = 1.0f;

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

    public class Node
    {
        public int id;
        public Rect Rect = new Rect(30, 30, 200, 100);

        public string title { get { return String.Format("Window {0}", id); } }

        public Vector2 contentOffset;

        public virtual void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Label("Simple Node!");

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Button("Info", EditorStyles.toolbarButton);
            GUILayout.Button("Metadata", EditorStyles.toolbarButton);
            GUILayout.EndHorizontal();

            GUILayout.TextField("Sample");

            GUILayout.EndVertical();
        }

        public virtual void DrawNodeWindow(State State)
        {
            Rect nodeRect = Rect;

            nodeRect.position += State.PanOffset;
            contentOffset = new Vector2(0, 20);

            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Label(headerRect, title, State.SelectedNode == this ? EditorStyles.boldLabel : EditorStyles.label);

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y - contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);

            GUI.changed = false;
            OnGUI();

            GUILayout.EndArea();
            GUI.EndGroup();
        }
    }

    public class Canvas
    {
        public State State;
        public InputSystem InputSystem;

        public void OnGUI()
        {
            InputSystem.HandleEvents(State, false);

            DrawConnections();
            DrawNodes();

            InputSystem.HandleEvents(State, true);
        }

        private void DrawConnections() { }

        private void DrawNodes()
        {
            for (int i = 0; i < State.Nodes.Count; i++)
            {
                State.Nodes[i].DrawNodeWindow(State);
            }
        }
    }

    public class NodeDialogueEditor : EditorWindow
    {
        private static NodeDialogueEditor _editor;
        public static NodeDialogueEditor editor { get { AssureEditor(); return _editor; } }
        public static void AssureEditor() { if (_editor == null) OpenEditor(); }

        private Texture2D _resizeHandle;

        private Canvas Canvas;

        private DialogueContainer Container;
        private string ContainerPath;

        [MenuItem("Window/Ashogue/Dialogue Editor")]
        public static NodeDialogueEditor OpenEditor()
        {
            _editor = EditorWindow.GetWindow<NodeDialogueEditor>();
            _editor.titleContent = new GUIContent("Dialogue Editor");

            return _editor;
        }

        private void Setup()
        {
            _resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/ResizeHandle.png", typeof(Texture2D)) as Texture2D;

            State state = new State
            {
                Nodes = new List<Node> {
                    new Node { id = 1 }
                }
            };

            state.Repaints -= Repaint;
            state.Repaints += Repaint;

            InputSystem IS = new InputSystem(typeof(InputControls));

            Canvas = new Canvas { InputSystem = IS, State = state };
        }

        private void OnGUI()
        {
            if (Canvas == null)
                Setup();

            Canvas.OnGUI();
        }
    }
}