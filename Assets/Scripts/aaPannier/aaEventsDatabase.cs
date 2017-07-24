using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class aaEventsDatabase : ScriptableObject
{
    [SerializeField]
    public string Name = "";

    [SerializeField]
    public List<aaEventHandler> Handlers = new List<aaEventHandler>();

    public void Handle(aaEvent @event) =>
        Handlers.ForEach(handler => handler.Execute(@event));
}