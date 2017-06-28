using UnityEngine;
using GrandCentral;

public class DialogueTest : MonoBehaviour, IInteractiveBehaviour
{
    [SerializeField]
    private string BirdType;

    public void OnLookEnter() { }
    public void OnLookStay() { }
    public void OnLookExit() { }

    public void OnInteract()
    {
        FactDictionary context = new FactDictionary();
        context.Add("bird", BirdType);
        context.Add("speaker", "protag");

        ContextAwareRequest.Request("seen-bird", context);
    }
}