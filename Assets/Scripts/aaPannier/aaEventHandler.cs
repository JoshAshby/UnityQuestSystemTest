using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class aaEventHandler
{
    public string EventName;
    [SerializeField]
    public List<aaCriterion> Criteria = new List<aaCriterion>();
    public int Padding = 0;

    [SerializeField]
    public List<aaResponse> Responses = new List<aaResponse>();

    public void Execute(aaEvent eventA)
    {
        Debug.Log(eventA.EventName);
    }
}