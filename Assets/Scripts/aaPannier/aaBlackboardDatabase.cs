using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

[XmlRoot("aaBlackboardDatabase")]
[Serializable]
public class aaBlackboardDatabase
{
    private Dictionary<string, aaSavableBlackboard> _blackboards;
    [XmlIgnore]
    public Dictionary<string, aaSavableBlackboard> Blackboards
    {
        get { return _blackboards; }
        set { _blackboards = value; }
    }

    [XmlArray("Blackboards")]
    [XmlArrayItem("aaSavableBlackboard", typeof(aaSavableBlackboard))]
    public aaSavableBlackboard[] XmlBlackboards
    {
        get { return Blackboards.Values.ToArray(); }
        set { Blackboards = value.ToDictionary(i => i.Hint, i => i); }
    }

    // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
    // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
    // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(aaBlackboardDatabase));
        using (var stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, this);
        }
    }

    public static aaBlackboardDatabase Load(string path)
    {
        var serializer = new XmlSerializer(typeof(aaBlackboardDatabase));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return (aaBlackboardDatabase)serializer.Deserialize(stream);
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static aaBlackboardDatabase LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(aaBlackboardDatabase));
        return (aaBlackboardDatabase)serializer.Deserialize(new StringReader(text));
    }

    public void SetFact(string hint, string key, object value)
    {
        Type type = value.GetType();

        if (type != typeof(bool) || type != typeof(int) || type != typeof(string))
        {
            Debug.LogError($"Can't set blackboard '{hint}' fact '{key}' with type {type}!");
            return;
        }

        if (!Blackboards.ContainsKey(hint))
            Blackboards.Add(hint, new aaSavableBlackboard());

        aaSavableBlackboard blackboard = Blackboards[hint];

        aaFact fact = new aaFact { Key = key, Type = type, Value = value };

        if (!blackboard.Facts.ContainsKey(key))
            blackboard.Facts.Add(key, fact);
        else
            blackboard.Facts[key] = fact;
    }

    public object GetFact(string hint, string key)
    {
        if (!Blackboards.ContainsKey(hint))
            Blackboards.Add(hint, new aaSavableBlackboard());

        aaSavableBlackboard blackboard = Blackboards[hint];

        if (!blackboard.Facts.ContainsKey(key))
        {
            aaFact fact = new aaFact { Key = key, Type = typeof(int), Value = 0 };
            blackboard.Facts.Add(key, fact);
        }

        return blackboard.Facts[key];
    }
}