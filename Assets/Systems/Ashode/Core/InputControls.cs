using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ashode
{
    public class GenericMenuCallbackData<T>
    {
        public InputEvent InputEvent { get; }
        public T Object { get; }

        public Vector2 CanvasSpaceMouse
        {
            get { return InputEvent.Canvas.ScreenToCanvasSpace((InputEvent.Event.mousePosition)) - InputEvent.State.GlobalCanvasSize.position; }
        }

        public GenericMenuCallbackData(InputEvent inputEvent, T action)
        {
            this.InputEvent = inputEvent;
            this.Object = action;
        }
    }

    // Default, example input event handlers
    public static class InputControls
    {
        private static bool updateFocus = false;

        // Hover on node
        [EventHandler(Priority = -4)]
        public static void HandleNodeFocus(InputEvent inputEvent)
        {
            inputEvent.State.FocusedNode = inputEvent.Control as INode;
            inputEvent.State.FocusedKnob = inputEvent.Control as IKnob;
            inputEvent.State.FocusedConnection = inputEvent.Control as IConnection;

            inputEvent.Canvas.OnRepaint();

            if (Event.current.type == EventType.Repaint && updateFocus)
            {
                updateFocus = false;
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
            }
        }

        [MouseEventHandler(Priority = -5)]
        [HotkeyHandler(KeyCode.Escape)]
        public static void HandleNullClick(InputEvent inputEvent)
        {
            if (inputEvent.Control != null && !inputEvent.Event.isKey)
                return;

            inputEvent.State.FocusedNode = null;
            inputEvent.State.SelectedNode = null;

            inputEvent.State.FocusedKnob = null;
            inputEvent.State.SelectedKnob = null;
            inputEvent.State.ExpandedKnob = null;
            inputEvent.State.ConnectedFromKnob = null;

            inputEvent.State.FocusedConnection = null;

            updateFocus = true;
        }

        // Click on connection
        [MouseEventHandler(HandledType = typeof(IConnection), Priority = -4)]
        public static void HandleConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IConnection connection = inputEvent.Control as IConnection;

            inputEvent.State.RemoveConnection(connection);
            inputEvent.Event.Use();
        }

        // Finish building a connection
        [MouseEventHandler(HandledType = typeof(IKnob), Priority = -3)]
        public static void HandleMakeConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IKnob knob = inputEvent.Control as IKnob;

            if (inputEvent.State.ConnectedFromKnob == null)
                return;

            if (!Connection.Verify(inputEvent.State.ConnectedFromKnob, knob))
                return;

            inputEvent.State.AddConnection(inputEvent.State.ConnectedFromKnob, knob);
            inputEvent.State.ExpandedKnob = inputEvent.State.ConnectedFromKnob;
            inputEvent.State.ConnectedFromKnob = null;

            inputEvent.Event.Use();
        }

        // Start building a connection
        [MouseEventHandler(HandledType = typeof(IKnob), Priority = -2)]
        public static void HandleStartConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IKnob knob = inputEvent.Control as IKnob;

            if (!knob.Available)
                return;

            if (inputEvent.State.ExpandedKnob != knob)
                return;

            inputEvent.State.ConnectedFromKnob = knob;
            inputEvent.State.SelectedKnob = null;
            inputEvent.State.ExpandedKnob = null;
            inputEvent.Event.Use();
        }

        // Click on knob
        [MouseEventHandler(HandledType = typeof(IKnob), Priority = -1)]
        public static void HandleKnobClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IKnob knob = inputEvent.Control as IKnob;

            inputEvent.State.SelectedKnob = knob;
            inputEvent.State.ExpandedKnob = knob;
            inputEvent.State.ConnectedFromKnob = null;

            inputEvent.Event.Use();
        }

        // Click on node
        [MouseEventHandler(HandledType = typeof(INode), Priority = 0)]
        public static void HandleNodeClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            INode node = inputEvent.Control as INode;

            if (node == inputEvent.State.SelectedNode)
                return;

            if (inputEvent.State.FocusedNode != node && inputEvent.State.FocusedNode != null)
            {
                updateFocus = true;
                inputEvent.Event.Use();
            }

            inputEvent.State.SelectedNode = node;
            inputEvent.State.SelectedKnob = null;
            inputEvent.State.ExpandedKnob = null;
            inputEvent.State.ConnectedFromKnob = null;

            inputEvent.Canvas.OnRepaint();
        }

        private static void AddNodeCallback(object obj)
        {
            GenericMenuCallbackData<Type> callbackEvent = obj as GenericMenuCallbackData<Type>;

            callbackEvent.InputEvent.State.AddNode(callbackEvent.Object, callbackEvent.CanvasSpaceMouse);
        }

        // Right click on canvas
        [MouseEventHandler(MouseButtons.Right, Priority = 2)]
        public static void HandleCanvasRightClick(InputEvent inputEvent)
        {
            if (inputEvent.Control != null)
                return;

            List<NodeBelongsToAttributeInfo> info = inputEvent.Canvas.NodeTypes();

            GenericMenu menu = new GenericMenu();
            foreach (var attr in info)
            {
                if (attr.Hidden)
                    continue;

                menu.AddItem(new GUIContent(String.Format("Add/{0}", attr.Name)), false, AddNodeCallback, new GenericMenuCallbackData<Type>(inputEvent, attr.NodeType));
            }
            menu.ShowAsContext();

            inputEvent.Event.Use();
        }

        private static void RemoveNodeCallback(object obj)
        {
            InputEvent inputEvent = obj as InputEvent;

            inputEvent.State.RemoveNode(inputEvent.Control as INode);
        }

        // Right click on node
        [MouseEventHandler(MouseButtons.Right, HandledType = typeof(INode), Priority = 1)]
        public static void HandleNodeRightClick(InputEvent inputEvent)
        {
            INode node = inputEvent.Control as INode;

            if(!node.Removable)
                return;

            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Remove Node"), false, RemoveNodeCallback, inputEvent);
            menu.ShowAsContext();

            inputEvent.Event.Use();
        }

        // Start dragging node
        [MouseEventHandler(Priority = 110)]
        public static void HandleNodeDragStart(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
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

        // Dragging node
        [MouseEventHandler(EventType.MouseDrag)]
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

            Rect newPos = new Rect(inputEvent.State.SelectedNode.Rect);
            newPos.position = inputEvent.State.DragPosition + inputEvent.State.DragOffset;

            inputEvent.State.SelectedNode.Rect = newPos;
            inputEvent.Canvas.OnRepaint();
        }

        // Stop dragging node
        [MouseEventHandler(EventType.MouseDown)]
        [MouseEventHandler(EventType.MouseUp)]
        public static void HandleNodeDragStop(InputEvent inputEvent)
        {
            inputEvent.State.Dragging = false;
        }

        // Start panning
        [MouseEventHandler(Priority = 100)]
        public static void HandlePanStart(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.State.FocusedNode != null)
                return;

            inputEvent.State.Panning = true;
            inputEvent.State.DraggingStart = inputEvent.Event.mousePosition;
            inputEvent.State.DragOffset = Vector2.zero;
        }

        // Drag panning
        [MouseEventHandler(EventType.MouseDrag)]
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
            panOffsetChange = (inputEvent.State.DragOffset - panOffsetChange);
            inputEvent.State.PanOffset += panOffsetChange;
            inputEvent.Canvas.OnRepaint();
        }

        // Stop Panning
        [MouseEventHandler(EventType.MouseDown)]
        [MouseEventHandler(EventType.MouseUp)]
        public static void HandlePanStop(InputEvent inputEvent)
        {
            inputEvent.State.Panning = false;
        }

        // Keyboard panning
        [HotkeyHandler(KeyCode.UpArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.DownArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.LeftArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.RightArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.W, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.S, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.A, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.D, EventType.KeyDown)]
        public static void HandleKeyboardNav(InputEvent inputEvent)
        {
            if (inputEvent.State.SelectedNode != null)
                return;

            int shiftAmount = inputEvent.Event.shift ? 100 : 25;
            switch (inputEvent.Event.keyCode)
            {
                case KeyCode.UpArrow:
                case KeyCode.W:
                    inputEvent.State.PanOffset += new Vector2(0, -shiftAmount);
                    break;

                case KeyCode.DownArrow:
                case KeyCode.S:
                    inputEvent.State.PanOffset += new Vector2(0, shiftAmount);
                    break;

                case KeyCode.LeftArrow:
                case KeyCode.A:
                    inputEvent.State.PanOffset += new Vector2(-shiftAmount, 0);
                    break;

                case KeyCode.RightArrow:
                case KeyCode.D:
                    inputEvent.State.PanOffset += new Vector2(shiftAmount, 0);
                    break;
            }

            inputEvent.Canvas.OnRepaint();
            inputEvent.Event.Use();
        }
    }
}