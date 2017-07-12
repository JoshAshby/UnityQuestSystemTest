using System;
using UnityEngine;

[Serializable]
public abstract class aaResponse
{
    public abstract void Execute(aaEvent @event);
    public abstract void OnCustomGUI();
    public abstract string GetDebugText();
}

[Serializable]
public class aaDebugResponse : aaResponse
{
    public override void Execute(aaEvent @event) =>
        Debug.Log($"EVENT LOG: {@event.EventName}");

    public override void OnCustomGUI() { }

    public override string GetDebugText() => $"";
}