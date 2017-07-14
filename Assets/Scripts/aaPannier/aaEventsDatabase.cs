using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class aaEventsDatabase : ScriptableObject
{
    public List<aaEventHandler> Handlers = new List<aaEventHandler>();

    public void Handle(aaEvent @event) =>
        Handlers.ForEach(handler => handler.Execute(@event));
}