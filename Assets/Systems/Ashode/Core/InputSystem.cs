using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Ashode
{
    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public EventType? EventType { get; set; }

        protected int _priority = 50;
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public EventHandlerAttribute() { this.EventType = null; }
        public EventHandlerAttribute(EventType type) { this.EventType = type; }
    }

    public enum MouseButtons
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }

    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
    public class MouseEventHandlerAttribute : Attribute
    {
        public EventType? EventType { get; set; }

        public MouseButtons Button { get; set; }

        protected int _priority = 50;
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public MouseEventHandlerAttribute() { this.Button = MouseButtons.Left; }

        public MouseEventHandlerAttribute(MouseButtons button) { this.Button = button; }

        public MouseEventHandlerAttribute(EventType type)
        {
            this.EventType = type;
            this.Button = MouseButtons.Left;
        }

        public MouseEventHandlerAttribute(MouseButtons button, EventType type)
        {
            this.EventType = type;
            this.Button = button;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
    public class HotkeyHandlerAttribute : Attribute
    {
        public KeyCode Key { get; set; }
        public EventModifiers? Modifiers { get; set; }
        public EventType? EventType { get; set; }

        protected int _priority = 50;
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public HotkeyHandlerAttribute(KeyCode key) { this.Key = key; }

        public HotkeyHandlerAttribute(KeyCode key, EventModifiers modifiers)
        {
            this.Key = key;
            this.Modifiers = modifiers;
        }

        public HotkeyHandlerAttribute(KeyCode key, EventType limiter)
        {
            this.Key = key;
            this.EventType = limiter;
        }

        public HotkeyHandlerAttribute(KeyCode key, EventModifiers modifiers, EventType limiter)
        {
            this.Key = key;
            this.Modifiers = modifiers;
            this.EventType = limiter;
        }
    }

    // Passed along to event handlers to encapsulate the current info
    public class InputEvent
    {
        public Event Event { get; }
        public EventType Type { get { return Event.type; } }

        public INodeCanvas Canvas { get; }
        public IState State { get { return Canvas.State; } }

        public IControl Control { get; }

        public InputEvent(Event Event, INodeCanvas Canvas)
        {
            this.Event = Event;
            this.Canvas = Canvas;

            Vector2 canvasSpace = Canvas.ScreenToCanvasSpace(Event.mousePosition);
            Control = Canvas.FindControlAt(canvasSpace);
        }
    }

    abstract class AttributeInfo
    {
        public EventType? EventType { get; internal set; }

        public int Priority { get; internal set; }

        public Action<InputEvent> Action { get; internal set; }
    }

    class EventAttributeInfo : AttributeInfo
    {
        public EventAttributeInfo(EventHandlerAttribute attribute, MethodInfo method)
        {
            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
        }
    }

    class MouseEventAttributeInfo : AttributeInfo
    {
        public int Button { get; internal set; }

        public MouseEventAttributeInfo(MouseEventHandlerAttribute attribute, MethodInfo method)
        {
            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);
            this.EventType = attribute.EventType;
            this.Button = (int)attribute.Button;
            this.Priority = attribute.Priority;
        }
    }

    class HotkeyAttributeInfo : AttributeInfo
    {
        public KeyCode Key { get; internal set; }
        public EventModifiers? Modifiers { get; internal set; }

        public HotkeyAttributeInfo(HotkeyHandlerAttribute attribute, MethodInfo method)
        {
            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);
            this.Key = attribute.Key;
            this.Modifiers = attribute.Modifiers;
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
        }
    }

    public class InputSystem
    {
        private List<EventAttributeInfo> EventAttributeInfos = new List<EventAttributeInfo>();
        private List<MouseEventAttributeInfo> MouseEventAttributeInfos = new List<MouseEventAttributeInfo>();
        private List<HotkeyAttributeInfo> HotkeyAttributeInfos = new List<HotkeyAttributeInfo>();

        public InputSystem(Type controlContainer)
        {
            AddHandlersFrom(controlContainer);
        }

        public void AddHandlersFrom(Type controlContainer)
        {
            MethodInfo[] methods = controlContainer.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
            foreach (var method in methods)
            {
                EventHandlerAttribute[] eventAttributes = method.GetCustomAttributes(typeof(EventHandlerAttribute), true) as EventHandlerAttribute[];
                foreach (var attribute in eventAttributes)
                {
                    EventAttributeInfos.Add(new EventAttributeInfo(attribute, method));
                }

                MouseEventHandlerAttribute[] mouseEventAttributes = method.GetCustomAttributes(typeof(MouseEventHandlerAttribute), true) as MouseEventHandlerAttribute[];
                foreach (var attribute in mouseEventAttributes)
                {
                    MouseEventAttributeInfos.Add(new MouseEventAttributeInfo(attribute, method));
                }

                HotkeyHandlerAttribute[] hotkeyAttributes = method.GetCustomAttributes(typeof(HotkeyHandlerAttribute), true) as HotkeyHandlerAttribute[];
                foreach (var attribute in hotkeyAttributes)
                {
                    HotkeyAttributeInfos.Add(new HotkeyAttributeInfo(attribute, method));
                }
            }

            EventAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            MouseEventAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            HotkeyAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void HandleEvents(NodeCanvas canvas, bool late)
        {
            InputEvent inputEvent = new InputEvent(Event.current, canvas);

            if (ShouldIgnoreEvent(inputEvent))
                return;

            HandlePlainEvents(inputEvent, late);
            HandleMouseEvents(inputEvent, late);
            HandleHotkeyEvents(inputEvent, late);
        }

        private bool ShouldIgnoreEvent(InputEvent inputEvent)
        {
            return !inputEvent.State.CanvasSize.Contains(inputEvent.Event.mousePosition);
        }

        private void HandlePlainEvents(InputEvent inputEvent, bool late)
        {
            var attrInfos = EventAttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Type);

            foreach (var info in attrInfos)
            {
                info.Action(inputEvent);

                if (inputEvent.Type == EventType.Used)
                    return;
            }
        }

        private void HandleMouseEvents(InputEvent inputEvent, bool late)
        {
            if (inputEvent.Type != EventType.MouseDown || inputEvent.Type != EventType.MouseUp)
                return;

            var attrInfos = MouseEventAttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Type)
                .Where(x => x.Button == inputEvent.Event.button);

            foreach (var info in attrInfos)
            {
                info.Action(inputEvent);

                if (inputEvent.Type == EventType.Used)
                    return;
            }
        }

        private void HandleHotkeyEvents(InputEvent inputEvent, bool late)
        {
            if (inputEvent.Type != EventType.KeyDown || inputEvent.Type != EventType.KeyUp)
                return;

            var attrInfos = HotkeyAttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.Key == inputEvent.Event.keyCode)
                .Where(x => x.Modifiers == null || x.Modifiers == inputEvent.Event.modifiers)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Type);

            foreach (var info in attrInfos)
            {
                info.Action(inputEvent);

                if (inputEvent.Type == EventType.Used)
                    return;
            }
        }
    }
}