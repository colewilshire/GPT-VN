using System.Collections.Generic;

public class NewDialogueController : Singleton<NewDialogueController>
{
    private List<NewDialogueLine> dialoguePath;
    public int CurrentLineIndex = 0;

    private void ReadDialogueLine(int lineIndex)
    {
        CurrentLineIndex = lineIndex;
        NewDialogueLine currentLine = dialoguePath[CurrentLineIndex];
        CharacterPortrait characterPortrait = NewCharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);
        NewBackgroundController.Instance.ShowBackground(currentLine.BackgroundDescription);
        NameDisplayController.Instance.SetDisplayName(currentLine.CharacterName);
        TextController.Instance.SetText(currentLine.DialogueText);
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
