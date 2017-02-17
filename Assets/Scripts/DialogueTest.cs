using UnityEngine;

public class DialogueTest : MonoBehaviour, IInteractiveBehaviour
{
    public void OnLookEnter() { }
    public void OnLookStay() { }
    public void OnLookExit() { }

    public void OnInteract()
    {
        Dialogue.Loader.Build();
    }
}