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
            string res2 = DialogueController.Instance.Rules.From("bird-cylinders", "seen-robin")
                .Where("speaker", "protag")
                .Where("bird", "robin")
                .Select();

            Debug.Log(res2);
        }
    }
}