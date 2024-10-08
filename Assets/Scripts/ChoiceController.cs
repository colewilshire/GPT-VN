public class ChoiceController : Singleton<ChoiceController>
{
    public Choice CurrentChoice {get; private set;}

    public void ShowChoices(Choice choice)
    {
        CurrentChoice = choice;
        StateController.Instance.SetSubmenuState(GameState.Choice);
    }

    public void MakeChoice(DialogueLine dialogueLine)
    {
        DialogueController.Instance.MakeChoice(dialogueLine);
    }
}
