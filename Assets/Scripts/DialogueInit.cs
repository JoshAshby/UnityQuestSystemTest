using UnityEngine;
using Ashogue;
using Zenject;

public class DialogueInit : MonoBehaviour
{
    private void Awake()
    {
        DialogueController.Initialize();
    }
}