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
            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test1", "beta").SelectAs<string>() == null);
            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test1", "alpha").SelectAs<string>() == "wat");

            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test2", 5).SelectAs<string>() == null);
            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test2", 6).SelectAs<string>() == "bat");

            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test3", 5).SelectAs<string>() == null);
            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test3", 13).SelectAs<string>() == null);
            Debug.Assert(DialogueController.Instance.Rules.From("test").Where("test3", 10).SelectAs<string>() == "mat");

            string res1 = DialogueController.Instance.Rules.From("test")
                .Where("test4", "robin")
                .Where("test5", 19)
                .Where("test6", 7)
                .SelectAs<string>();
            Debug.Assert(res1 == null);

            string res2 = DialogueController.Instance.Rules.From("test")
                .Where("test4", "robin")
                .Where("test5", 19)
                .Where("test6", 3)
                .SelectAs<string>();
            Debug.Assert(res2 == "hat");
            Debug.Log(res2);
        }
    }
}