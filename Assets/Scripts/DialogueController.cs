using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class DialogueController : Singleton<DialogueController>
{
    private List<DialogueLine> DialoguePath = new List<DialogueLine>();
    private int currentLineIndex = 0;

    private void ReadDialogueLine(int index)
    {
        DialogueLine currentLine = DialoguePath[index];

        BackgroundController.Instance.SetBackground(currentLine.backgroundImage);
        CharacterManager.Instance.ShowPortait(currentLine.characterName, currentLine.mood);
        NameDisplayController.Instance.SetDisplayName(currentLine.characterName);
        TextController.Instance.SetText(currentLine.dialogueText);
        AudioController.Instance.PlaySound(currentLine.voiceLine);
    }

    private DialogueLine DeserializeLine(string serializedLine)
    {
        List<string> splitLine = new List<string>(serializedLine.Split('|', StringSplitOptions.RemoveEmptyEntries))
            .Select(str => str.Trim())
            .Select(str => str.Trim('"'))
            .ToList();
        DialogueLine dialogueLine = DialogueLine.CreateInstance<DialogueLine>();
        string characterName = splitLine[0];
        string dialogueText = splitLine[1];
        string characterMood = splitLine[2];
        string backgroundName = splitLine[3];

        dialogueLine.characterName = characterName;
        dialogueLine.dialogueText = dialogueText;
        dialogueLine.mood = characterMood;
        dialogueLine.backgroundImage = BackgroundController.Instance.GetBackgroundImageFromName(backgroundName);

        return dialogueLine;
    }

    public void StepForward()
    {
        if (!(DialoguePath.Count > currentLineIndex + 1)) return;
        currentLineIndex += 1;
        ReadDialogueLine(currentLineIndex);
    }

    public void StepBackward()
    {
        if (!(currentLineIndex - 1 >= 0)) return;
        currentLineIndex -= 1;
        ReadDialogueLine(currentLineIndex);
    }

    public void RepeatLine()
    {
        if (!(DialoguePath.Count > 0)) return;
        ReadDialogueLine(currentLineIndex);
    }

    public void StartDialogue(string serializedDialouge)
    {
        DialoguePath = new List<DialogueLine>();

        AddToDialogue(serializedDialouge);
        ReadDialogueLine(currentLineIndex);
    }

    public void AddToDialogue(string serializedDialouge)
    {
        string[] serializedLines = serializedDialouge.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string serializedLine in serializedLines)
        {
            DialogueLine dialogueLine = DeserializeLine(serializedLine);
            DialoguePath.Add(dialogueLine);
        }
    }

}