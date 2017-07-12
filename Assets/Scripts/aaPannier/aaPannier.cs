using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Prefab("aaPannier")]
class aaPannier : Singleton<aaPannier>
{
    public List<aaEventsDatabase> EventDatabases;

    public static void Publish(string EventName, GameObject Target=null, GameObject Sender=null)
    {
        Instance.PublishEvent(EventName, Target: Target, Sender: Sender);
    }

    public void PublishEvent(string EventName, GameObject Target=null, GameObject Sender=null)
    {
        aaEvent eventA = new aaEvent{ EventName = EventName, Target = Target, Sender = Sender};

        foreach(var db in EventDatabases)
        {
            foreach(var handle in db.Handlers)
            {
                if(EventName != handle.EventName)
                    continue;

                handle.Execute(eventA);
            }
        }
    }

    private void Awake()
    {
        EventDatabases = Resources.LoadAll<aaEventsDatabase>("EventDatabases/").ToList();
    }
}