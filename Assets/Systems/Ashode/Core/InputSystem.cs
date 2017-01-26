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
        public Type HandledType { get; set; }

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
        public Type HandledType { get; set; }

        public MouseButtons Button { get; set; }

        protected int _priority = 50;
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public MouseEventHandlerAttribute()
        {
            this.Button = MouseButtons.Left;
            this.EventType = UnityEngine.EventType.MouseDown;
        }

        public MouseEventHandlerAttribute(MouseButtons button)
        {
            this.Button = button;
            this.EventType = UnityEngine.EventType.MouseDown;
        }

        public MouseEventHandlerAttribute(EventType type)
        {
            this.Button = MouseButtons.Left;
            this.EventType = type;
        }

        public MouseEventHandlerAttribute(MouseButtons button, EventType type)
        {
            this.Button = button;
            this.EventType = type;
        }
    }

    [AttributeUsageAttribute(AttributeTargets.Method, AllowMultiple = true)]
    public class HotkeyHandlerAttribute : Attribute
    {
        public EventType? EventType { get; set; }
        public Type HandledType { get; set; }

        public KeyCode Key { get; set; }
        public EventModifiers? Modifiers { get; set; }

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

    class AttributeInfo
    {
        public EventType? EventType { get; internal set; }
        public int Priority { get; internal set; }

        public Action<InputEvent> Action { get; internal set; }

        public Type HandledType { get; internal set; }

        public int? Button { get; internal set; }

        public KeyCode? Key { get; internal set; }
        public EventModifiers? Modifiers { get; internal set; }

        public AttributeInfo(EventHandlerAttribute attribute, MethodInfo method)
        {
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
            this.HandledType = attribute.HandledType;

            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);
        }

        public AttributeInfo(MouseEventHandlerAttribute attribute, MethodInfo method)
        {
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
            this.HandledType = attribute.HandledType;

            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);

            this.Button = (int)attribute.Button;
        }

        public AttributeInfo(HotkeyHandlerAttribute attribute, MethodInfo method)
        {
            this.EventType = attribute.EventType;
            this.Priority = attribute.Priority;
            this.HandledType = attribute.HandledType;

            this.Action = (Action<InputEvent>)Delegate.CreateDelegate(typeof(Action<InputEvent>), method);

            this.Key = attribute.Key;
            this.Modifiers = attribute.Modifiers;
        }
    }

    public class InputSystem
    {
        private List<AttributeInfo> AttributeInfos = new List<AttributeInfo>();

        public InputSystem(params Type[] controlContainers)
        {
            foreach(var controlContainer in controlContainers)
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
                    AttributeInfos.Add(new AttributeInfo(attribute, method));
                }

                MouseEventHandlerAttribute[] mouseEventAttributes = method.GetCustomAttributes(typeof(MouseEventHandlerAttribute), true) as MouseEventHandlerAttribute[];
                foreach (var attribute in mouseEventAttributes)
                {
                    AttributeInfos.Add(new AttributeInfo(attribute, method));
                }

                HotkeyHandlerAttribute[] hotkeyAttributes = method.GetCustomAttributes(typeof(HotkeyHandlerAttribute), true) as HotkeyHandlerAttribute[];
                foreach (var attribute in hotkeyAttributes)
                {
                    AttributeInfos.Add(new AttributeInfo(attribute, method));
                }
            }

            AttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        private bool ShouldIgnoreEvent(InputEvent inputEvent)
        {
            return !inputEvent.State.LocalCanvasSize.Contains(inputEvent.Event.mousePosition);
        }

        public void HandleEvents(NodeCanvas canvas, bool late)
        {
            InputEvent inputEvent = new InputEvent(Event.current, canvas);

            if (ShouldIgnoreEvent(inputEvent))
                return;

            Type controlType = null;
            if (inputEvent.Control != null)
                controlType = inputEvent.Control.GetType();

            var attrInfos = AttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Type)
                .Where(x => x.Modifiers == null || x.Modifiers == inputEvent.Event.modifiers)
                .Where(x => x.HandledType == null || x.HandledType.IsAssignableFrom(controlType));

            foreach (var info in attrInfos)
            {
                if (info.Button != null && inputEvent.Event.button != info.Button)
                    continue;

                if (info.Key != null && inputEvent.Event.keyCode != info.Key)
                    continue;

                info.Action(inputEvent);

                if (inputEvent.Type == EventType.Used)
                    return;
            }
        }
    }
}