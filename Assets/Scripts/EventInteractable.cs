using UnityEngine;

public class EventInteractable : MonoBehaviour, IInteractiveBehaviour
{
    [SerializeField]
    private string EventName = "OnInteract";

    public void OnLookEnter() { }
    public void OnLookStay() { }
    public void OnLookExit() { }

    public void OnInteract()
    {
        aaPannier.Publish(EventName, Target: this.gameObject);
    }
}