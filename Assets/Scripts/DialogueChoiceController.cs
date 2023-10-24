public class DialogueChoiceController : Singleton<DialogueChoiceController>
{
    public DialogueChoice CurrentChoice {get; private set;}

    public void ShowChoices(DialogueChoice dialogueChoice)
    {
        CurrentChoice = dialogueChoice;
        StateController.Instance.SetSubmenuState(GameState.Choice);
    }

    public void MakeChoice(DialogueLine dialogueLine)
    {
        DialogueController.Instance.AddToDialogue(dialogueLine.SerializedLine);
        OpenAIController.Instance.GenerateAdditionalDialogue(dialogueLine.DialogueText);
    }
}
