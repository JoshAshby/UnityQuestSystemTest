using UnityEngine;
using UnityEngine.UI;

using GrandCentral;

public class DialogueChoiceButton : MonoBehaviour
{
    private Text _text = null;
    private string _key = null;

    private void Awake()
    {
        _text = GetComponentInChildren<Text>();
    }

    public void Setup(IDialogueChoice choice)
    {
        _text.text = choice.Label;
        _key = choice.Key;
    }
}