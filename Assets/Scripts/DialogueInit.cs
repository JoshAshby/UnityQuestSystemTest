using UnityEngine;
using Ashogue;

public class DialogueInit : MonoBehaviour
{
    private void Awake()
    {
        DialogueController.Initialize();
    }
}