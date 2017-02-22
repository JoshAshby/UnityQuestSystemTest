using UnityEngine;

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
            StateShard context = new StateShard();
            context.Add("speaker", "protag");
            context.Add("listener", "");
            context.Add("bird", BirdType);

            string res = DialogueController.Instance.QueryFor("bird-cylinders", "seen-robin", context);

            Debug.Log(res);
        }
    }
}