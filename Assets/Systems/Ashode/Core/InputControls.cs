using System;
using UnityEngine;

namespace Ashode
{
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

        // Click on connection
        [MouseEventHandler( Priority = -3)]
        public static void HandleConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IConnection connection = inputEvent.Control as IConnection;

            if (connection == null)
                return;

            inputEvent.State.RemoveConnection(connection);
            inputEvent.Event.Use();
        }

        // Finish building a connection
        [MouseEventHandler(Priority = -2)]
        public static void HandleMakeConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IKnob knob = inputEvent.Control as IKnob;

            if (knob == null)
                return;

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
        [MouseEventHandler(Priority = -1)]
        public static void HandleStartConnectionClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            IKnob knob = inputEvent.Control as IKnob;

            if (knob == null)
                return;

            if (!knob.Available)
                return;

            if (inputEvent.State.ExpandedKnob != knob)
                return;

            inputEvent.State.ConnectedFromKnob = knob;
            inputEvent.State.SelectedKnob = null;
            inputEvent.State.ExpandedKnob = null;
            inputEvent.Event.Use();
        }

        // Click on node
        [MouseEventHandler(Priority = -0)]
        public static void HandleNodeClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            INode node = inputEvent.Control as INode;
            IKnob knob = inputEvent.Control as IKnob;

            if(node == inputEvent.State.SelectedNode && node != null)
                return;

            if (inputEvent.State.FocusedNode != node && inputEvent.State.FocusedNode != null)
                updateFocus = true;
                // inputEvent.Event.Use();

            inputEvent.State.SelectedNode = node;
            inputEvent.State.SelectedKnob = knob;
            inputEvent.State.ExpandedKnob = knob;
            inputEvent.State.ConnectedFromKnob = null;

            Debug.Log(inputEvent.State.SelectedNode);

            inputEvent.Canvas.OnRepaint();
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

            if (inputEvent.Event.button != 0)
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