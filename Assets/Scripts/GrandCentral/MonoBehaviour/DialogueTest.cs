using UnityEngine;

namespace GrandCentral
{
    public class DialogueTest : MonoBehaviour, IInteractiveBehaviour
    {
        public void OnLookEnter() { }
        public void OnLookStay() { }
        public void OnLookExit() { }

        public void OnInteract()
        {
            int cylinders = StateController.Instance.State["player"]["cylinders-seen"].OfType<int>().Value;

            string res2 = DialogueController.Instance.Rules.From("bird-cylinders")
                .Where("bird", "robin")
                .Select();

            StateController.Instance.State["player"]["cylinders-seen"].OfType<int>().Value++;

            Debug.Log(res2);
        }
    }
}