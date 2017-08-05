using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

[Serializable]
public class aaSavableBlackboard
{
    public string Hint;

    private Dictionary<string, aaFact> _facts;
    [XmlIgnore]
    public Dictionary<string, aaFact> Facts
    {
        get { return _facts; }
        set { _facts = value; }
    }

    [XmlArray("Facts")]
    [XmlArrayItem("aaFact", typeof(aaFact))]
    public aaFact[] XmlFacts
    {
        get { return Facts.Values.ToArray(); }
        set { Facts = value.ToDictionary(i => i.Key, i => i); }
    }
}