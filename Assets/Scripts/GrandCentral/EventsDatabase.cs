using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;

namespace GrandCentral
{
    public class EventEntry
    {
        private string _eventName;
        [XmlAttribute("EventName")]
        public string EventName
        {
            get { return _eventName; }
            set { _eventName = value; }
        }

        private List<IEntry> _entries = new List<IEntry>();
        [XmlIgnore]
        public List<IEntry> Entries
        {
            get { return _entries; }
            set { _entries = value; }
        }

        [XmlArray("Entries")]
        [XmlArrayItem("Entry", typeof(Entry))]
        public IEntry[] XmlEntries
        {
            get { return Entries.ToArray(); }
            set { Entries = value.ToList<IEntry>(); }
        }

        public IEntry QueryFor(Blackboard QueryContext, BlackboardsContainer BlackboardsContainer)
        {
            List<IEntry> entries = Entries.Where(ent => ent.Check(QueryContext, BlackboardsContainer)).ToList();

            IEntry entry;
            int length = entries.Count();

            if (length <= 1)
                entry = entries.FirstOrDefault();
            else
            {
                entries = entries
                          .Where(ent => ent.Length == entries.First().Length)
                          .ToList();

                int count = entries.Count();
                int index = Random.Range(0, count);

                entry = entries.ElementAtOrDefault(index);
            }

            if (entry == null)
                return null;

            entry.BlackboardMutations.ForEach(x => x.Mutate(BlackboardsContainer));

            return entry;
        }
    }

    [XmlRoot("EventsDatabase")]
    public class EventsDatabase
    {
        private Dictionary<string, EventEntry> _eventRules = new Dictionary<string, EventEntry>();
        [XmlIgnore]
        public Dictionary<string, EventEntry> EventRules
        {
            get { return _eventRules; }
            set { _eventRules = value; }
        }

        [XmlArray("EventEntries")]
        [XmlArrayItem("EventEntry", typeof(EventEntry))]
        public EventEntry[] XmlEventRules
        {
            get { return EventRules.Values.ToArray(); }
            set { EventRules = value.ToDictionary(i => i.EventName, i => i); }
        }

        // Copypasta from http://wiki.unity3d.com/index.php?title=Saving_and_Loading_Data:_XmlSerializer
        // var monsterCollection = MonsterContainer.Load(Path.Combine(Application.dataPath, "monsters.xml"));
        // monsterCollection.Save(Path.Combine(Application.persistentDataPath, "monsters.xml"));
        public void Save(string path)
        {
            var serializer = new XmlSerializer(typeof(EventsDatabase));
            using (var stream = new FileStream(path, FileMode.Create))
            {
                serializer.Serialize(stream, this);
            }
        }

        public static EventsDatabase Load(string path)
        {
            var serializer = new XmlSerializer(typeof(EventsDatabase));
            using (var stream = new FileStream(path, FileMode.Open))
            {
                return (EventsDatabase)serializer.Deserialize(stream);
            }
        }

        //Loads the xml directly from the given string. Useful in combination with www.text.
        public static EventsDatabase LoadFromText(string text)
        {
            var serializer = new XmlSerializer(typeof(EventsDatabase));
            return (EventsDatabase)serializer.Deserialize(new StringReader(text));
        }

        public IEntry QueryFor(string EventName, Blackboard QueryContext, BlackboardsContainer BlackboardsContainer)
        {
            return Get(EventName).QueryFor(QueryContext, BlackboardsContainer);
        }

        private EventEntry Get(string EventName)
        {
            if (!EventRules.ContainsKey(EventName))
                EventRules[EventName] = new EventEntry { EventName = EventName };

            return EventRules[EventName];
        }
    }
}