using System;
using System.Collections.Generic;
using System.Linq;
using GrandCentral.Telegraph;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Telegraph Controller", true)]
    public class TelegraphController : Singleton<TelegraphController>
    {
        protected IDictionary<Type, IList<object>> _subscribers;

        public static void Publish<T>(T message) where T : IMessage
        {
            Instance._Publish<T>(message);
        }

        public static void Subscribe(object subscriber)
        {
            Instance._Subscribe(subscriber);
        }

        public static void Unsubscribe(object subscriber)
        {
            Instance._Unsubscribe(subscriber);
        }

        private void Awake()
        {
            _subscribers = new Dictionary<Type, IList<object>>();
        }

        private void _Publish<T>(T message) where T : IMessage
        {
            if (!_subscribers.ContainsKey(typeof(T)) || !_subscribers[typeof(T)].Any())
            {
                Debug.LogWarning(string.Format("Message {0} has no subscribers", typeof(T).FullName));
                return;
            }

            foreach (var handler in _subscribers[typeof(T)])
                ((IHandle<T>)handler).Handle(message);
        }

        private List<Type> GetHandledMessageTypes(object subscriber)
        {
            return subscriber.GetType()
                .GetInterfaces()
                .Where(handler => handler.IsGenericType && handler.GetGenericTypeDefinition() == typeof(IHandle<>))
                .Select(handler => handler.GetGenericArguments().Single())
                .ToList();
        }

        private void _Subscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => _Subscribe(message, subscriber));
        }

        private void _Subscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType))
                _subscribers[messageType] = new List<object>();

            _subscribers[messageType].Add(subscriber);
        }

        private void _Unsubscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => _Unsubscribe(message, subscriber));
        }

        private void _Unsubscribe(Type messageType, object subscriber)
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