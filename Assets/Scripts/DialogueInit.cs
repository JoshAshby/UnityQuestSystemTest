using UnityEngine;
using Ashogue;
using Zenject;

public class DialogueInit : MonoBehaviour
{
    [Inject]
    private DialogueController DialogueController;
     
    private void Awake()
    {
        DialogueController.Initialize();
    }
}