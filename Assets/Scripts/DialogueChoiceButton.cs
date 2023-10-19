using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueChoiceButton : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button button;
    private DialogueLine dialogueLine;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }

    private void OnClick()
    {
        ChoiceController.Instance.MakeChoice(dialogueLine);
    }

    public void SetDialogueLine(DialogueLine newDialogueLine)
    {
        dialogueLine = newDialogueLine;
        dialogueText.text = dialogueLine.DialogueText.ToLower();
    }
}
