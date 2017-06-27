using UnityEngine;
using GrandCentral.Facts;

namespace GrandCentral
{
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

            DialogueDisplay.RequestLine("protag", "seen-robin", context);
        }
    }
}