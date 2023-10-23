using UnityEngine;

public class ChoiceController : Singleton<ChoiceController>
{
    [SerializeField] private DialogueChoiceButton choiceButtonPrefab;

    private void DestroyChoices()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ShowChoices(DialogueChoice dialogueChoice)
    {
        foreach (DialogueLine dialogueLine in dialogueChoice.Choices)
        {
            DialogueChoiceButton choice = Instantiate(choiceButtonPrefab, transform);

            choice.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            choice.SetDialogueLine(dialogueLine);
        }
    }

    public void MakeChoice(DialogueLine dialogueLine)
    {
        DestroyChoices();
        //DialogueController.Instance.ContinueDialogue(dialogueLine.SerializedLine);
        DialogueController.Instance.AddToDialogue(dialogueLine.SerializedLine);
        OpenAIController.Instance.GenerateAdditionalDialogue(dialogueLine.DialogueText);
    }
}
