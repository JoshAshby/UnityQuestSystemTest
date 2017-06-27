using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral.Events
{
    public static class EventsController
    {
        private static IDictionary<Type, IList<object>> _subscribers = new Dictionary<Type, IList<object>>();

        public static void Publish<T>(T message) where T : IEvent
        {
            if (!_subscribers.ContainsKey(typeof(T)) || !_subscribers[typeof(T)].Any())
            {
                Debug.LogWarning(string.Format("Message {0} has no subscribers", typeof(T).FullName));
                return;
            }

            foreach (var handler in _subscribers[typeof(T)])
                ((IHandle<T>)handler).Handle(message);
        }

        public static void Subscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => _Subscribe(message, subscriber));
        }

        public static void Unsubscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => _Unsubscribe(message, subscriber));
        }

        private static List<Type> GetHandledMessageTypes(object subscriber)
        {
            return subscriber.GetType()
                .GetInterfaces()
                .Where(handler => handler.IsGenericType && handler.GetGenericTypeDefinition() == typeof(IHandle<>))
                .Select(handler => handler.GetGenericArguments().Single())
                .ToList();
        }

        private static void _Subscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType))
                _subscribers[messageType] = new List<object>();

            _subscribers[messageType].Add(subscriber);
        }

        private static void _Unsubscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType) || !_subscribers[messageType].Contains(subscriber))
            {
                Debug.LogWarning(string.Format("Unsubscribing {0} but it was never subscribed", subscriber.GetType().FullName));
                return;
            }

            _subscribers[messageType].Remove(subscriber);
        }
    }
}