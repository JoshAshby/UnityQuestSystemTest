using UnityEngine;

using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using System.IO;

namespace GrandCentral
{
    public class Blackboard
    {
        private string _hint;
        [XmlAttribute("Hint")]
        public string Hint
        {
            get { return _hint; }
            set { _hint = value; }
        }

        private Dictionary<string, object> _facts = new Dictionary<string, object>();
        [XmlIgnore]
        public Dictionary<string, object> Facts
        {
            get { return _facts; }
            set { _facts = value; }
        }

        [XmlArray("Data")]
        [XmlArrayItem("KeyValuePair", typeof(KeyValuePair<string, object>))]
        public KeyValuePair<string, object>[] XmlData
        {
            get { return Facts.ToArray(); }
            set { Facts = value.ToDictionary(i => i.Key, i => i.Value); }
        }

        public bool ContainsKey(string FactKey)
        {
            return Facts.ContainsKey(FactKey);
        }

        public T Get<T>(string FactKey)
        {
            if (!Facts.ContainsKey(FactKey))
                Facts[FactKey] = default(T);

            return (T)Facts[FactKey];
        }

        public void Set<T>(string FactKey, T value)
        {
            Facts[FactKey] = value;
        }

        public void Remove(string FactKey)
        {
            Facts.Remove(FactKey);
        }

        public object this[string FactKey]
        {
            get { return Get<object>(FactKey); }
            set { Set<object>(FactKey, value); }
        }
    }

    [XmlRoot("BlackboardsContainer")]
    public class BlackboardsContainer
    {
        private Dictionary<string, Blackboard> _blackboards = new Dictionary<string, Blackboard>();
        [XmlIgnore]
        public Dictionary<string, Blackboard> Blackboards
        {
            get { return _blackboards; }
            set { _blackboards = value; }
        }

        [XmlArray("Blackboards")]
        [XmlArrayItem("Blackboard", typeof(Blackboard))]
        public Blackboard[] XmlBlackboards
        {
            get { return Blackboards.Values.ToArray(); }
            set { Blackboards = value.ToDictionary(i => i.Hint, i => i); }
        }

        // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
        // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
        // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(BlackboardsContainer));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static BlackboardsContainer Load(string path)
        {
            var serializer = new XmlSerializer(typeof(BlackboardsContainer));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (BlackboardsContainer)serializer.Deserialize(stream);
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static BlackboardsContainer LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(BlackboardsContainer));
            return (BlackboardsContainer)serializer.Deserialize(new StringReader(text));
        }

        public T Get<T>(string Hint, string FactKey)
        {
            return GetBlackboard(Hint).Get<T>(FactKey);
        }

        public void Set<T>(string Hint, string FactKey, T value)
        {
            GetBlackboard(Hint).Set<T>(FactKey, value);
        }

        public void Remove(string Hint, string FactKey)
        {
            GetBlackboard(Hint).Remove(FactKey);
        }

        private Blackboard GetBlackboard(string Hint)
        {
            if (!Blackboards.ContainsKey(Hint))
                Blackboards.Add(Hint, new Blackboard());

            return Blackboards[Hint];
        }
    }
}