using UnityEngine;
using GrandCentral;

public class EventInteractable : MonoBehaviour, IInteractiveBehaviour
{
    [SerializeField]
    private string EventName = "OnInteract";

    public void OnLookEnter() { }
    public void OnLookStay() { }
    public void OnLookExit() { }

    public void OnInteract()
    {
        Pannier.Publish(EventName);
    }
}