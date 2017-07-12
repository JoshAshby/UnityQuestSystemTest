using System;
using UnityEngine;

[Serializable]
public abstract class aaCriterion
{
    public string Hint;
    public string FactKey;

    public abstract bool Check(aaEvent @event);
}