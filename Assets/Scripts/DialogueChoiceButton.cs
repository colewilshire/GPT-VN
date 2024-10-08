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
        if (dialogueLine != null)
        {
            DialogueController.Instance.MakeChoice(dialogueLine);
        }
    }

    public void SetDialogueLine(DialogueLine DialogueLine)
    {
        dialogueLine = DialogueLine;
        dialogueText.text = dialogueLine.DialogueText.ToLower();
    }
}
