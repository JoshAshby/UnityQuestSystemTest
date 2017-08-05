using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Prefab("aaPannier")]
class aaPannier : Singleton<aaPannier>
{
    public List<aaEventsDatabase> EventDatabases;
    public aaBlackboardDatabase BlackboardDatabase;

    public string SavePath;

    public static void Publish(string EventName, GameObject Target = null, GameObject Sender = null) =>
            Instance.PublishEvent(EventName, Target: Target, Sender: Sender);

    public void PublishEvent(string EventName, GameObject Target = null, GameObject Sender = null)
    {
        aaEvent @event = new aaEvent { EventName = EventName, Target = Target, Sender = Sender };

        EventDatabases.ForEach(db => db.Handle(@event));
    }

    // TODO: Make this handle loading only the inital ones marked for load
    // And handle dynamic unloading/loading per scene
    private void Awake()
    {
        SavePath = Path.Combine(Application.dataPath, "DataFiles/saves/save01.xml");
        Debug.Log($"Saving to {SavePath}");

        EventDatabases = Resources.LoadAll<aaEventsDatabase>("EventDatabases/").ToList();

        if (File.Exists(SavePath))
            BlackboardDatabase = aaBlackboardDatabase.Load(SavePath);
        else
            BlackboardDatabase = new aaBlackboardDatabase();
    }

    private void OnDestory()
    {
        BlackboardDatabase.Save(SavePath);
    }
}