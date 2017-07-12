using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu]
public class aaEventsDatabase : ScriptableObject
{
    public List<aaEventHandler> Handlers;
}