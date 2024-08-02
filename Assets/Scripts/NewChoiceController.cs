public class NewChoiceController : Singleton<NewChoiceController>
{
    public Choice CurrentChoice {get; private set;}

    public void ShowChoices(Choice choice)
    {
        CurrentChoice = choice;
        StateController.Instance.SetSubmenuState(GameState.Choice);
    }

    public void MakeChoice(NewDialogueLine dialogueLine)
    {
        NewDialogueController.Instance.MakeChoice(dialogueLine);
    }
}
