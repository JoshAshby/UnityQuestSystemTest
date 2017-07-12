using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Prefab("aaPannier")]
class aaPannier : Singleton<aaPannier>
{
    public List<aaEventsDatabase> EventDatabases;

    public static void Publish(string EventName, GameObject Target = null, GameObject Sender = null)
    {
        Instance.PublishEvent(EventName, Target: Target, Sender: Sender);
    }

    public void PublishEvent(string EventName, GameObject Target = null, GameObject Sender = null)
    {
        aaEvent @event = new aaEvent { EventName = EventName, Target = Target, Sender = Sender };

        foreach (var db in EventDatabases)
        {
            foreach (var handle in db.Handlers)
            {
                handle.Execute(@event);
            }
        }
    }

    // TODO: Make this handle loading only the inital ones marked for load
    // And handle dynamic unloading/loading per scene
    private void Awake() =>
        EventDatabases = Resources.LoadAll<aaEventsDatabase>("EventDatabases/").ToList();
}