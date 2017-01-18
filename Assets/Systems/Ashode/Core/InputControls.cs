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
            Vector2 canvasSpace = inputEvent.Canvas.ScreenToCanvasSpace(inputEvent.Event.mousePosition);

            INode node = null;
            IKnob knob = null;

            inputEvent.Canvas.FindNodeOrKnobAt(canvasSpace, out node, out knob);

            inputEvent.State.FocusedNode = node;
            inputEvent.State.FocusedKnob = knob;

            if (Event.current.type == EventType.Repaint && updateFocus)
            {
                updateFocus = false;
                GUIUtility.hotControl = 0;
                GUIUtility.keyboardControl = 0;
            }
        }

        // Click on knob
        [EventHandler(EventType.MouseDown, Priority = -3)]
        public static void HandleKnobClick(InputEvent inputEvent)
        {
            if(inputEvent.State.SelectedKnob == null)
                return;

            if(inputEvent.State.FocusedKnob == null)
                return;

            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.Event.button != 0)
                return;

            if (inputEvent.State.FocusedKnob == inputEvent.State.SelectedKnob)
                return;

            if(inputEvent.State.FocusedKnob.Type != inputEvent.State.SelectedKnob.Type)
                return;

            Vector2 canvasSpace = inputEvent.Canvas.ScreenToCanvasSpace(inputEvent.Event.mousePosition);

            INode node = null;
            IKnob knob = null;

            inputEvent.Canvas.FindNodeOrKnobAt(canvasSpace, out node, out knob);

            inputEvent.State.Connections.Add(new Connection(inputEvent.State.SelectedKnob, knob));

            inputEvent.State.FocusedKnob = null;
            inputEvent.State.SelectedKnob = knob;
            inputEvent.State.FocusedNode = null;
            inputEvent.State.SelectedNode = node;

            inputEvent.Canvas.OnRepaint();
        }

        // Click on node
        [EventHandler(EventType.MouseDown, Priority = -2)]
        public static void HandleNodeClick(InputEvent inputEvent)
        {
            if (GUIUtility.hotControl > 0)
                return;

            if (inputEvent.Event.button != 0)
                return;

            if (inputEvent.State.FocusedNode == inputEvent.State.SelectedNode && inputEvent.State.FocusedNode != null)
                return;

            Vector2 canvasSpace = inputEvent.Canvas.ScreenToCanvasSpace(inputEvent.Event.mousePosition);

            INode node = null;
            IKnob knob = null;

            inputEvent.Canvas.FindNodeOrKnobAt(canvasSpace, out node, out knob);

            inputEvent.State.SelectedNode = node;
            inputEvent.State.SelectedKnob = knob;

            updateFocus = true;
            inputEvent.Canvas.OnRepaint();
        }

        // Drag node
        [EventHandler(EventType.MouseDown, Priority = 110)]
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

        [EventHandler(EventType.MouseDrag)]
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

        [EventHandler(EventType.MouseDown)]
        [EventHandler(EventType.MouseUp)]
        public static void HandleNodeDragStop(InputEvent inputEvent)
        {
            inputEvent.State.Dragging = false;
        }

        // Panning
        [EventHandler(EventType.MouseDown, Priority = 100)]
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

        [EventHandler(EventType.MouseDrag)]
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

        [EventHandler(EventType.MouseDown)]
        [EventHandler(EventType.MouseUp)]
        public static void HandlePanStop(InputEvent inputEvent)
        {
            inputEvent.State.Panning = false;
        }

        // Keyboard panning
        [HotkeyHandler(KeyCode.UpArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.DownArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.LeftArrow, EventType.KeyDown)]
        [HotkeyHandler(KeyCode.RightArrow, EventType.KeyDown)]
        public static void HandleKeyboardNav(InputEvent inputEvent)
        {
            if (inputEvent.State.SelectedNode != null)
                return;

            int shiftAmount = inputEvent.Event.shift ? 100 : 25;
            switch (inputEvent.Event.keyCode)
            {
                case KeyCode.UpArrow:
                    inputEvent.State.PanOffset += new Vector2(0, -shiftAmount);
                    break;

                case KeyCode.DownArrow:
                    inputEvent.State.PanOffset += new Vector2(0, shiftAmount);
                    break;

                case KeyCode.LeftArrow:
                    inputEvent.State.PanOffset += new Vector2(-shiftAmount, 0);
                    break;

                case KeyCode.RightArrow:
                    inputEvent.State.PanOffset += new Vector2(shiftAmount, 0);
                    break;
            }

            inputEvent.Canvas.OnRepaint();
            inputEvent.Event.Use();
        }
    }
}