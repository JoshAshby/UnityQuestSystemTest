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
            int cylinders = (int)StateController.Instance.State["player"]["cylinders-seen"];

            string res2 = DialogueController.Instance.Rules.From("bird-cylinders")
                .Where("bird", "robin")
                .Select();

            StateController.Instance.State["player"]["cylinders-seen"] = cylinders + 1;

            Debug.Log(res2);
        }
    }
}