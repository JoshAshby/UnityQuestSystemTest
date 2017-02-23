using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GrandCentral.Telegraph
{
    public interface IMessage { }

    public interface IHandle<T> where T : IMessage
    {
        void Handle(T message);
    }

    public interface IMessenger
    {
        void Subscribe(object subscriber);
        void Unsubscribe(object subscriber);
        void Publish<T>(T message) where T : IMessage;
    }

    public class Messenger : IMessenger
    {
        readonly IDictionary<Type, IList<object>> _subscribers = new Dictionary<Type, IList<object>>();

        public void Publish<T>(T message) where T : IMessage
        {
            if (!_subscribers.ContainsKey(typeof(T)) || !_subscribers[typeof(T)].Any())
            {
                Debug.LogWarning(string.Format("Message {0} has no subscribers", typeof(T).FullName));
                return;
            }

            foreach (var handler in _subscribers[typeof(T)])
                ((IHandle<T>)handler).Handle(message);
        }

        protected List<Type> GetHandledMessageTypes(object subscriber)
        {
            return subscriber.GetType()
                .GetInterfaces()
                .Where(handler => handler.IsGenericType && handler.GetGenericTypeDefinition() == typeof(IHandle<>))
                .Select(handler => handler.GetGenericArguments().Single())
                .ToList();
        }

        public void Subscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => Subscribe(message, subscriber));
        }

        void Subscribe(Type messageType, object subscriber)
        {
            if (!_subscribers.ContainsKey(messageType))
                _subscribers[messageType] = new List<object>();

            _subscribers[messageType].Add(subscriber);
        }

        public void Unsubscribe(object subscriber)
        {
            GetHandledMessageTypes(subscriber).ForEach(message => Unsubscribe(message, subscriber));
        }

        void Unsubscribe(Type messageType, object subscriber)
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