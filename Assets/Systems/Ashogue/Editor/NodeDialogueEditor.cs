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

    public static class InputSystem
    {
        public static void Setup(Type controlContainer)
        { 
            MethodInfo[] methods = controlContainer.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach(var method in methods)
            {
                EventHandlerAttribute[] attributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), true) as EventHandlerAttribute[];
            }
        }

        public static void HandleEvents(bool late)
        {

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
        [EventHandlerAttribute(-50)]
        public static void HandleNodeFocus(InputEvent inputEvent)
        {
            Debug.Log(inputEvent.State.FindNodeAt(inputEvent.State.ScreenToCanvasSpace(Event.current.mousePosition)));
        }

        [EventHandlerAttribute(EventType.MouseDown)]
        public static void HandleNodeClick(InputEvent inputEvent)
        {
            Debug.Log(inputEvent.State.FindNodeAt(inputEvent.State.ScreenToCanvasSpace(Event.current.mousePosition)));
        }
    }

    public class State
    {
        public List<Node> Nodes = new List<Node> { new Node { id = 1 } };

        public Node SelectedNode = null;
        public Node FocusedNode = null;

        public Vector2 PanOffset = Vector2.zero;

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
        public Rect Rect = new Rect(30, 30, 400, 400);

        public string title { get { return String.Format("Window {0}", id); } }

        public Vector2 contentOffset;

        public void OnGUI(State State)
        {
            Rect nodeRect = Rect;

            nodeRect.position += State.PanOffset;
            contentOffset = new Vector2(0, 20);

            Rect headerRect = new Rect(nodeRect.x, nodeRect.y, nodeRect.width, contentOffset.y);
            GUI.Label(headerRect, title);

            Rect bodyRect = new Rect(nodeRect.x, nodeRect.y - contentOffset.y, nodeRect.width, nodeRect.height - contentOffset.y);
            GUI.BeginGroup(bodyRect, GUI.skin.box);
            bodyRect.position = Vector2.zero;
            GUILayout.BeginArea(bodyRect, GUI.skin.box);

            GUI.changed = false;
            DrawNodeWindow();

            GUILayout.EndArea();
            GUI.EndGroup();
        }

        public void DrawNodeWindow()
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
    }

    public class Canvas
    {
        public State State = new State();

        public void OnGUI()
        {
            InputSystem.HandleEvents(false);

            DrawConnections();
            DrawNodes();

            InputSystem.HandleEvents(true);
        }

        private void DrawConnections() { }

        private void DrawNodes()
        {
            for (int i = 0; i < State.Nodes.Count; i++)
            {
                State.Nodes[i].OnGUI(State);
            }
        }
    }

    public class NodeDialogueEditor : EditorWindow
    {
        private static NodeDialogueEditor _editor;
        public static NodeDialogueEditor editor { get { AssureEditor(); return _editor; } }
        public static void AssureEditor() { if (_editor == null) OpenEditor(); }

        private Texture2D _resizeHandle;

        private Canvas Canvas = new Canvas();

        private DialogueContainer Container;
        private string ContainerPath;

        [MenuItem("Window/Ashogue/Dialogue Editor")]
        public static NodeDialogueEditor OpenEditor()
        {
            _editor = EditorWindow.GetWindow<NodeDialogueEditor>();

            _editor.titleContent = new GUIContent("Dialogue Editor");

            return _editor;
        }

        private void Awake()
        {
            _resizeHandle = AssetDatabase.LoadAssetAtPath("Assets/ResizeHandle.png", typeof(Texture2D)) as Texture2D;
        }

        private void OnGUI()
        {
            Canvas.OnGUI();
        }
    }
}