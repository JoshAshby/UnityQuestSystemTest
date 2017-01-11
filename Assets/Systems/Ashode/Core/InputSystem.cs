using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;

namespace Ashode
{
    public class InputEvent
    {
        public Event Event;

        public Canvas Canvas;
        public State State { get { return Canvas.State; } }
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
        }
    }

    public class InputSystem
    {
        private List<EventAttributeInfo> EventAttributeInfos = new List<EventAttributeInfo>();
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

                HotkeyHandlerAttribute[] hotkeyAttributes = method.GetCustomAttributes(typeof(HotkeyHandlerAttribute), true) as HotkeyHandlerAttribute[];
                foreach (var attribute in hotkeyAttributes)
                {
                    HotkeyAttributeInfos.Add(new HotkeyAttributeInfo(attribute, method));
                }
            }

            EventAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            HotkeyAttributeInfos.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }

        public void HandleEvents(Canvas canvas, bool late)
        {
            InputEvent inputEvent = new InputEvent { Event = Event.current, Canvas = canvas };

            if (ShouldIgnoreEvent(inputEvent))
                return;

            HandlePlainEvents(inputEvent, late);
            HandleHotkeyEvents(inputEvent, late);
        }

        private bool ShouldIgnoreEvent(InputEvent inputEvent)
        {
            return false;
        }

        private void HandlePlainEvents(InputEvent inputEvent, bool late)
        {
            var attrInfos = EventAttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Event.type);

            foreach (var info in attrInfos)
            {
                info.Action(inputEvent);

                if (inputEvent.Event.type == EventType.Used)
                    return;
            }
        }

        private void HandleHotkeyEvents(InputEvent inputEvent, bool late)
        {
            var attrInfos = HotkeyAttributeInfos
                .Where(x => late ? x.Priority >= 100 : x.Priority < 100)
                .Where(x => x.Key == inputEvent.Event.keyCode)
                .Where(x => x.Modifiers == null || x.Modifiers == inputEvent.Event.modifiers)
                .Where(x => x.EventType == null || x.EventType == inputEvent.Event.type);

            foreach (var info in attrInfos)
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

        protected int _priority = 50;
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public EventHandlerAttribute() { this.EventType = null; }
        public EventHandlerAttribute(EventType type) { this.EventType = type; }
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
}