using UnityEngine;
using System;

namespace Ashode
{
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
            inputEvent.State.Repaint();
        }

        [EventHandler(EventType.MouseDown)]
        [EventHandler(EventType.MouseUp)]
        public static void HandleNodeDragStop(InputEvent inputEvent)
        {
            inputEvent.State.Dragging = false;
        }

        // Panning
        [EventHandler(EventType.MouseDown, 100)]
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
            inputEvent.State.Repaint();
        }

        [EventHandler(EventType.MouseDown)]
        [EventHandler(EventType.MouseUp)]
        public static void HandlePanStop(InputEvent inputEvent)
        {
            inputEvent.State.Panning = false;
        }
    }
}