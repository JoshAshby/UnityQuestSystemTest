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
            int cylinders = StateController.Instance.State["test"]["cylinders"].OfType<int>().Value;

            string res2 = DialogueController.Instance.Rules.From("test")
                .Where("bird", "robin")
                .Where("cylinders-seen", cylinders)
                .SelectAs<string>();

            StateController.Instance.State["test"]["cylinders"].OfType<int>().Value++;

            Debug.Log(res2);
        }
    }
}