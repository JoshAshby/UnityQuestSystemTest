using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GrandCentral
{
    [Prefab("Pannier")]
    public class Pannier : Singleton<Pannier>
    {
        [SerializeField]
        private string BlackboardSaveLocation = "GrandCentral/blackboards.xml";
        [SerializeField]
        private string EventsDatabaseSaveLocation = "GrandCentral/events.xml";

        public BlackboardsContainer BlackboardsContainer = new BlackboardsContainer();
        public EventsDatabase EventsDatabase = new EventsDatabase();

        public static IEntry Publish(string EventName, Blackboard QueryContext=null)
        {
            return Instance.PublishEvent(EventName, QueryContext);
        }

        public IEntry PublishEvent(string EventName, Blackboard QueryContext=null)
        {
            IEntry entry = EventsDatabase.QueryFor(EventName, QueryContext, BlackboardsContainer);

            if (entry != null)
                EventBus.Publish<ResponseEvent>(new ResponseEvent { Entry = entry });

            return entry;
        }

        private void Awake()
        {
            // BlackboardsContainer = BlackboardsContainer.Load(Path.Combine(Application.dataPath, BlackboardSaveLocation));
            // EventsDatabase = EventsDatabase.Load(Path.Combine(Application.dataPath, EventsDatabaseSaveLocation));
        }

        private void OnDestroy()
        {
            BlackboardsContainer.Save(Path.Combine(Application.dataPath, BlackboardSaveLocation));
            EventsDatabase.Save(Path.Combine(Application.dataPath, EventsDatabaseSaveLocation));
        }
    }
}