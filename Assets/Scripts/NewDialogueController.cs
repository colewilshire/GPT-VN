using System.Collections.Generic;

public class NewDialogueController : Singleton<NewDialogueController>
{
    private List<NewDialogueLine> dialoguePath;
    public int CurrentLineIndex = 0;
    public Choice CurrentChoice;

    private void ReadDialogueLine(int lineIndex)
    {
        CurrentLineIndex = lineIndex;
        NewDialogueLine currentLine = dialoguePath[CurrentLineIndex];
        CharacterPortrait characterPortrait = NewCharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);
        NewBackgroundController.Instance.ShowBackground(currentLine.BackgroundDescription);
        NameDisplayController.Instance.SetDisplayName(currentLine.CharacterName);
        TextController.Instance.SetText(currentLine.DialogueText);

        if (currentLine.Choice != null)
        {
            ReadChoices(currentLine.Choice);
        }
        else
        {
            HideChoices();
        }
    }

    private void ReadChoices(Choice choice)
    {
        CurrentChoice = choice;
        StateController.Instance.SetSubmenuState(GameState.Choice);
    }

    private void HideChoices()
    {
        CurrentChoice = null;
        StateController.Instance.SetSubmenuState(GameState.Gameplay);
    }

    public async void MakeChoice(NewDialogueLine chosenLine)
    {
        dialoguePath[CurrentLineIndex] = chosenLine;
        CurrentChoice = null;

        StateController.Instance.SetStates(GameState.Loading);
        DialogueScene newDialogueScene = await NewOpenAIController.Instance.GenerateAdditionalDialogue(chosenLine.DialogueText);
        AddSceneToDialogue(newDialogueScene);

        StateController.Instance.SetStates(GameState.Gameplay);
        RepeatLine();
    }

    public void AddSceneToDialogue(DialogueScene dialogueScene)
    {
        foreach (NewDialogueLine dialogueLine in dialogueScene.DialogueLines)
        {
            dialoguePath.Add(dialogueLine);
        }
    }

    public void StartDialogue(DialogueScene initialDialogueScene, int startingLineIndex = 0)
    {
        dialoguePath = new();

        AddSceneToDialogue(initialDialogueScene);
        ReadDialogueLine(startingLineIndex);
    }

    public void StepForward()
    {
        int newIndex = CurrentLineIndex + 1;
        if (!(dialoguePath.Count > newIndex)) return;
        ReadDialogueLine(newIndex);
    }

    public void StepBackward()
    {
        int newIndex = CurrentLineIndex - 1;
        if (!(newIndex >= 0)) return;
        ReadDialogueLine(newIndex);
    }

    public void RepeatLine()
    {
        if (!(dialoguePath.Count > 0)) return;
        ReadDialogueLine(CurrentLineIndex);
    }
}
