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
}