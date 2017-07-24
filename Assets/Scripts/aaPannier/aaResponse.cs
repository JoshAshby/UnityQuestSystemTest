using System;
using UnityEngine;

[Serializable]
public abstract class aaResponse : ScriptableObject
{
    public abstract void Execute(aaEvent @event);
    public abstract void OnCustomGUI();
    public abstract string GetDebugText();
}