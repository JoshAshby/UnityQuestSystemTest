using System;
using System.Linq;
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

    public int Length
    {
        get { return Criteria.Count + Padding; }
    }

    public void Execute(aaEvent @event)
    {
        if (@event.EventName != EventName)
            return;

        if (!Criteria.TrueForAll(criterion => criterion.Check(@event)))
            return;

        Responses.ForEach(response => response.Execute(@event));
    }

    public override string ToString() =>
        $"aaEventHandler(Score: {Length}, Criteria: {Criteria.Count}, Padding: {Padding}, Responses: {Responses.Count})";
}