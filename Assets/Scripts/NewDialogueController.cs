using System.Collections.Generic;

public class NewDialogueController : Singleton<NewDialogueController>
{
    private int currentLineIndex = 0;
    private List<NewDialogueLine> dialoguePath;

    private void ReadDialogueLine(int lineIndex)
    {
        currentLineIndex = lineIndex;
        NewDialogueLine currentLine = dialoguePath[currentLineIndex];
        CharacterPortrait characterPortrait = NewCharacterManager.Instance.ShowPortrait(currentLine.CharacterName, currentLine.Mood);

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
        int newIndex = currentLineIndex + 1;
        if (!(dialoguePath.Count > newIndex)) return;
        ReadDialogueLine(newIndex);
    }

    public void StepBackward()
    {
        int newIndex = currentLineIndex - 1;
        if (!(newIndex >= 0)) return;
        ReadDialogueLine(newIndex);
    }

    public void RepeatLine()
    {
        if (!(dialoguePath.Count > 0)) return;
        ReadDialogueLine(currentLineIndex);
    }
}
